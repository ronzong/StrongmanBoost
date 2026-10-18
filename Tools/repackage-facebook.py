"""修改外部配置后，无需 Unity 即可更新 FB ZIP 和本地预览。"""
import argparse
import datetime
import hashlib
import json
from pathlib import Path
import re
import zipfile

repo = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--config', type=Path, default=repo / 'Tools/facebook-config.json')
args = parser.parse_args()
config = json.loads(args.config.read_text(encoding='utf-8-sig'))
if not isinstance(config.get('appId'), str) or not re.fullmatch(r'\d+', config['appId']):
    raise ValueError('appId must be a numeric string')
for key in ('interstitialPlacementId', 'rewardedPlacementId'):
    if not isinstance(config.get(key), str) or not re.fullmatch(config['appId'] + r'_\d+', config[key]):
        raise ValueError('Invalid ' + key)
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
print(package)
print('SHA256:', info['zipSha256'])
