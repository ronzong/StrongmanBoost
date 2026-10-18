const fs = require('node:fs');
const path = require('node:path');
const http = require('node:http');
const assert = require('node:assert/strict');
const {chromium} = require('C:/Users/busin/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');
const site = path.resolve('../site');
const mime = {'.html':'text/html','.js':'application/javascript','.wasm':'application/wasm','.json':'application/json'};
const server = http.createServer((req,res)=>{
  if(req.url==='/favicon.ico'){res.writeHead(204);res.end();return;}
  let file=path.resolve(site,'.'+decodeURIComponent(req.url.split('?')[0]));
  if(!file.startsWith(site+path.sep)){res.writeHead(403);res.end();return;}
  if(fs.existsSync(file)&&fs.statSync(file).isDirectory())file=path.join(file,'index.html');
  if(!fs.existsSync(file)){res.writeHead(404);res.end();return;}
  res.setHeader('Content-Type',mime[path.extname(file)]||'application/octet-stream');fs.createReadStream(file).pipe(res);
});
(async()=>{
  fs.mkdirSync('Logs',{recursive:true});
  await new Promise(resolve=>server.listen(8150,'127.0.0.1',resolve));
  let browser,page;
  const errors=[],failed=[],external=[],logs=[];
  try{
    browser=await chromium.launch({headless:true,executablePath:'C:/Program Files/Google/Chrome/Application/chrome.exe',args:['--use-angle=swiftshader','--enable-unsafe-swiftshader']});
    const context=await browser.newContext({viewport:{width:540,height:960},hasTouch:true});
    await context.addInitScript(()=>{
      window.__test={mode:'success',rewarded:0,interstitial:0,placements:[]};
      window.confirm=message=>{
        if(message.includes('激励'))__test.rewarded++;else __test.interstitial++;
        return __test.mode!=='cancel';
      };
    });
    page=await context.newPage();
    await context.route('**/local-fb-sdk.js',route=>route.fulfill({contentType:'application/javascript',body:fs.readFileSync('Tools/local-fb-sdk.js','utf8')+`
      for(const name of ['getInterstitialAdAsync','getRewardedVideoAsync']){
        const original=FBInstant[name]; FBInstant[name]=async id=>{__test.placements.push({name,id});return original(id);};
      }` }));
    page.on('pageerror',e=>errors.push(e.message));
    page.on('console',m=>{logs.push(m.text());if(m.type()==='error')errors.push(m.text());});
    page.on('requestfailed',r=>failed.push(r.url()));
    page.on('request',r=>{if(!r.url().startsWith('http://127.0.0.1:8150')&&!r.url().startsWith('blob:')&&!r.url().startsWith('data:'))external.push(r.url());});
    await page.goto('http://127.0.0.1:8150/StrongmanBoost/');
    await page.waitForFunction(()=>window.strongmanBoostPlatformReady,{},{timeout:180000});
    await page.waitForTimeout(5000);
    assert.equal(await page.evaluate(()=>strongmanBoostConfig.appId),'994549686727845');
    assert.deepEqual((await page.evaluate(()=>__test.placements)).map(x=>x.id).sort(),[
      '994549686727845_994616933387787','994549686727845_994617010054446'].sort());
    await page.screenshot({path:'Logs/facebook-start.png'});
    const cdp=await context.newCDPSession(page);
    const tap=async(x,y)=>{
      await cdp.send('Input.dispatchTouchEvent',{type:'touchStart',touchPoints:[{x,y}]});
      await page.waitForTimeout(120);
      await cdp.send('Input.dispatchTouchEvent',{type:'touchEnd',touchPoints:[]});
      await page.waitForTimeout(1200);
    };
    const pref=key=>page.evaluate(key=>localStorage.getItem(strongmanBoostSaveKey+':'+key),key);
    await tap(235,735);
    assert.equal(await pref('HealthAmount'),'35','Touch training should grant original strength');
    await page.waitForFunction(()=>Number(localStorage.getItem(strongmanBoostSaveKey+':PlayerScaleX'))>6.8);
    const scale=Number(await pref('PlayerScaleX'));assert.ok(scale>1,'Growth should finish and persist');
    await page.screenshot({path:'Logs/facebook-playing.png'});
    await page.reload();await page.waitForFunction(()=>window.strongmanBoostPlatformReady);await page.waitForTimeout(2000);
    assert.equal(await pref('HealthAmount'),'35');assert.equal(Number(await pref('PlayerScaleX')),scale);
    await tap(235,735);assert.equal(await pref('HealthAmount'),'70','Restored game must continue playing');
    await tap(45,480);assert.equal(await pref('Cash'),'70');assert.equal(await pref('HealthAmount'),'0');
    assert.equal(await page.evaluate(()=>__test.interstitial),1);
    await tap(235,735);await tap(45,480);
    assert.equal(await pref('Cash'),'105');assert.equal(await page.evaluate(()=>__test.interstitial),1,'Interstitial cooldown');
    await page.evaluate(()=>__test.mode='cancel');
    await tap(50,755);assert.equal(await pref('Cash'),'105','Cancelled reward must grant nothing');
    assert.equal(await page.evaluate(()=>__test.rewarded),1,'Reward button must reach the SDK');
    await page.evaluate(()=>__test.mode='success');
    await tap(50,755);assert.equal(await pref('Cash'),'155','Completed reward grants 50 coins');
    await page.evaluate(()=>{unityInstance.SendMessage('GameManager','ShowRewardVideo');unityInstance.SendMessage('GameManager','ShowRewardVideo');});
    await page.waitForTimeout(2000);assert.equal(await pref('Cash'),'205','Concurrent reward requests grant once');
    await tap(35,120);assert.equal(await pref('Sound'),'','Sound setting saved');
    await page.reload();await page.waitForFunction(()=>window.strongmanBoostPlatformReady);await page.waitForTimeout(2000);
    assert.equal(await pref('Cash'),'205');assert.equal(await pref('Sound'),'');
    await tap(35,120);assert.equal(await pref('Sound'),null,'Sound can be enabled after restore');
    // 使用正常激励完成回调辅助积累升级费用，再实际触摸购买下一装备。
    while(Number(await pref('Cash'))<1650){
      const expected=Number(await pref('Cash'))+50;
      await page.evaluate(()=>unityInstance.SendMessage('GameManager','ShowRewardVideo'));
      await page.waitForFunction(expected=>Number(localStorage.getItem(strongmanBoostSaveKey+':Cash'))===expected,expected);
      await page.waitForTimeout(200);
    }
    // 原游戏在力量卖出时刷新购买按钮的可用性。
    await tap(235,735);await tap(45,480);
    const beforePurchase=Number(await pref('Cash'));
    await tap(435,860);assert.equal(await pref('ButtonsCount'),'1');
    assert.equal(Number(await pref('Cash')),beforePurchase-1650,'Upgrade uses original coin price');
    await page.reload();await page.waitForFunction(()=>window.strongmanBoostPlatformReady);await page.waitForTimeout(2000);
    assert.equal(await pref('ButtonsCount'),'1');
    await page.screenshot({path:'Logs/facebook-upgrade.png'});
    await tap(235,735);assert.equal(await pref('HealthAmount'),'65','Purchased item remains usable after reload');
    await tap(45,620);await page.waitForTimeout(2500);
    const autoHealth=Number(await pref('HealthAmount'));assert.ok(autoHealth>65,'Automatic training should run');
    await tap(45,620);const pausedHealth=Number(await pref('HealthAmount'));await page.waitForTimeout(1600);
    assert.equal(Number(await pref('HealthAmount')),pausedHealth,'Automatic training can stop');
    await tap(35,35);await page.waitForTimeout(3000);await tap(235,735);
    assert.ok(Number(await pref('HealthAmount'))>pausedHealth,'Scene restart restores and continues gameplay');
    await page.setViewportSize({width:390,height:844});await page.waitForTimeout(800);
    await page.screenshot({path:'Logs/facebook-mobile.png'});
    // 同一份 wasm/data 下，覆盖网页配置确认 C# 请求新的广告位；不修改交付配置。
    const alternate={appId:'123456789',interstitialPlacementId:'123456789_111',rewardedPlacementId:'123456789_222'};
    await context.route('**/facebook-config.json',route=>route.fulfill({json:alternate}));
    await page.reload();await page.waitForFunction(()=>window.strongmanBoostPlatformReady);await page.waitForTimeout(2000);
    await page.waitForFunction(()=>__test.placements.length>=2,{},{timeout:30000});
    await page.waitForTimeout(1000);
    assert.deepEqual((await page.evaluate(()=>__test.placements)).map(x=>x.id).sort(),['123456789_111','123456789_222']);
    assert.equal(await pref('Cash'),null,'Changing app ID isolates saves');
    await page.evaluate(()=>unityInstance.SendMessage('GameManager','ShowRewardVideo'));await page.waitForTimeout(1500);
    assert.equal(await pref('Cash'),'50','Reward still works with changed external IDs');
    await page.goto('http://127.0.0.1:8150/StrongmanBoost/?ads=nofill');
    await page.waitForFunction(()=>window.strongmanBoostPlatformReady);await page.waitForTimeout(1500);
    await page.waitForFunction(()=>__test.placements.length>=2,{},{timeout:30000});
    await page.evaluate(()=>unityInstance.SendMessage('GameManager','ShowRewardVideo'));await page.waitForTimeout(1000);
    assert.equal(await pref('Cash'),'50','No fill must not grant rewards');
    await context.unroute('**/facebook-config.json');
    const report={errors,failed,external,logs,save:await page.evaluate(()=>Object.fromEntries(Object.entries(localStorage).filter(([key])=>key.startsWith(strongmanBoostSaveKey))))};
    fs.writeFileSync('Logs/facebook-web-report.json',JSON.stringify(report,null,2));
    assert.deepEqual(errors,[]);assert.deepEqual(failed,[]);assert.deepEqual(external,[]);
    console.log('PASS: startup, touch training/sell, reward cancellation/once, cooldown, reload/continue, sound, auto training, scene restart, mobile, no fill, changed external IDs without recompilation');
    await context.close();
  }catch(error){
    fs.writeFileSync('Logs/facebook-web-failure.json',JSON.stringify({message:error.message,errors,logs,failed,external,state:page?await page.evaluate(()=>({test:__test,save:Object.fromEntries(Object.entries(localStorage))})):null},null,2));
    if(page)await page.screenshot({path:'Logs/facebook-failure.png'});
    throw error;
  }finally{if(browser)await browser.close();await new Promise(resolve=>server.close(resolve));}
})().catch(e=>{console.error(e);process.exitCode=1;});
