"""修改外部配置后，无需 Unity 即可更新 FB ZIP 和本地预览。"""
import argparse
import datetime
import hashlib
import json
from pathlib import Path
import re
import zipfile

import sys


def validate_config(config):
    if not isinstance(config.get('appId'), str) or not re.fullmatch(r'[0-9]+', config['appId']):
        raise ValueError('App ID must contain digits only.')
    for key in ('interstitialPlacementId', 'rewardedPlacementId'):
        if not isinstance(config.get(key), str) or not re.fullmatch(config['appId'] + r'_[0-9]+', config[key]):
            raise ValueError(key + ' must be a complete placement ID starting with App ID + "_".')


def input_config(current):
    print('StrongmanBoost FB 重新打包（无需重新编译 Unity）')
    print('回车保留当前值；Ctrl+C 取消。')
    result = dict(current)
    while True:
        result['appId'] = input('App ID [' + str(current.get('appId', '')) + ']: ').strip() or current.get('appId', '')
        if re.fullmatch(r'[0-9]+', result['appId']):
            break
        print('App ID 必须为纯数字，请重新输入。')
    for key, label in [('interstitialPlacementId', '插屏 ID'), ('rewardedPlacementId', '激励 ID')]:
        while True:
            result[key] = input(label + ' [' + str(current.get(key, '')) + ']: ').strip() or current.get(key, '')
            if re.fullmatch(result['appId'] + r'_[0-9]+', result[key]):
                break
            print('广告位必须是完整 ID，并以 ' + result['appId'] + '_ 开头，请重新输入。')
    return result


def repackage(repo, config, config_file, save_source=False):
    validate_config(config)
    web = repo / 'Builds/Facebook/web'
    preview = repo.parent / 'site/StrongmanBoost'
    info_file = repo / 'Tools/facebook-build-info.json'
    manifest_file = preview / 'build-info.json'
    if not (web / 'index.html').is_file() or not manifest_file.is_file():
        raise RuntimeError('Build and export the game with Unity first')
    info = json.loads(info_file.read_text(encoding='utf-8-sig'))
    manifest = json.loads(manifest_file.read_text(encoding='utf-8-sig'))
    text = json.dumps(config, indent=2) + '\n'
    # 只更新固定的本游戏配置路径，其他游戏和二进制资源均不改动。
    (web / 'facebook-config.json').write_text(text, encoding='utf-8')
    (preview / 'facebook-config.json').write_text(text, encoding='utf-8')
    package = web.parent / ('StrongmanBoost-Facebook-' + config['appId'] + '.zip')
    temporary = package.with_suffix('.zip.tmp')
    with zipfile.ZipFile(temporary, 'w', zipfile.ZIP_DEFLATED) as archive:
        for file in sorted(web.rglob('*')):
            if not file.is_file():
                continue
            relative = file.relative_to(web).as_posix()
            if relative.startswith('ButDontShipItWithYourGame') or relative.endswith('.symbols.json'):
                continue
            archive.write(file, relative)
    temporary.replace(package)
    now = datetime.datetime.now(datetime.timezone.utc).isoformat()
    info.update(config)
    info.update(zipPath=package.relative_to(repo).as_posix(), zipBytes=package.stat().st_size,
                zipSha256=hashlib.sha256(package.read_bytes()).hexdigest(), repackagedAtUtc=now,
                uploaded=False, publishedPreview=False, localValidation=[],
                realFacebookValidation='Pending manual Facebook platform testing')
    info_file.write_text(json.dumps(info, indent=2) + '\n', encoding='utf-8')
    manifest.update(appId=config['appId'], repackagedAtUtc=now)
    for entry in manifest['files']:
        if entry['path'] == 'facebook-config.json':
            entry['sha256'] = hashlib.sha256((preview / entry['path']).read_bytes()).hexdigest()
    manifest_file.write_text(json.dumps(manifest, indent=2) + '\n', encoding='utf-8')
    if save_source:
        config_file.write_text(text, encoding='utf-8')
    print('ZIP:', package)
    print('SHA256:', info['zipSha256'])


def main():
    repo = Path(__file__).resolve().parents[1]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--config', type=Path, default=repo / 'Tools/facebook-config.json')
    parser.add_argument('--interactive', action='store_true', help='输入三个 ID，成功打包后保存为后续构建默认配置')
    args = parser.parse_args()
    config = json.loads(args.config.read_text(encoding='utf-8-sig'))
    if args.interactive:
        config = input_config(config)
    repackage(repo, config, args.config, args.interactive)
    if args.interactive:
        print('完成：配置、FB ZIP 和本地预览已更新。未上传或发布。')


if __name__ == '__main__':
    try:
        main()
    except (KeyboardInterrupt, EOFError):
        print('已取消，未完成重新打包。', file=sys.stderr)
        sys.exit(1)
    except (OSError, ValueError, RuntimeError, zipfile.BadZipFile) as error:
        print('重新打包失败：' + str(error), file=sys.stderr)
        sys.exit(1)
