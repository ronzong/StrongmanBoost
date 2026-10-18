const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const callbacks = [];
const cache = new Map();
const context = {
  LibraryManager: { library: {} },
  mergeInto: Object.assign,
  UTF8ToString: value => value,
  callbackPtr: (id, status, result) => callbacks.push({ id, status, result }),
  FBInstant: {},
  IGPlugin: {
    objectCache: cache,
    addToCache: (id, ad) => cache.set(id, ad),
    getFromCache: id => cache.get(id),
    deferCallback: (fn, id, status, message) => fn(id, status, message)
  }
};
let source = fs.readFileSync('StrongmanBoost/Assets/Meta.InstantGames/Runtime/Plugins/InstantGamesPluginJSLib.jslib', 'utf8');
source = source.replace(/\{\{\{\s*makeDynCall\([^}]+\}\}\}/g, 'callbackPtr');
vm.runInNewContext(source, context);
const api = context.LibraryManager.library;
const call = async (id, name, args = []) => {
  await api.FBInstant_function(id, context.callbackPtr, name,
    JSON.stringify({ args: args.map(value => ({ type: 'string', value })) }), 200, 400);
  return callbacks.pop();
};
const adCall = async (id, name, adId) => {
  await api.FBInstant_ad_function(name, adId, '', id, context.callbackPtr, 200, 400);
  return callbacks.pop();
};
(async () => {
  assert.equal((await call(1, 'unsupported')).status, 400, 'Missing API must reject');
  context.FBInstant.getRewardedVideoAsync = async placementID => ({
    getPlacementID: () => placementID,
    loadAsync: async () => {},
    showAsync: async () => {}
  });
  let result = await call(2, 'getRewardedVideoAsync', ['1606637210931767_1606903077571847']);
  assert.equal(result.status, 200);
  let ad = JSON.parse(result.result);
  assert.equal(ad.$2, 'REWARDED_VIDEO');
  assert.equal(ad.$1.placementID, '1606637210931767_1606903077571847');
  assert.equal((await adCall(3, 'loadAsync', ad.$1.adInstanceID)).status, 200);
  assert.equal((await adCall(4, 'showAsync', ad.$1.adInstanceID)).status, 200);
  assert.equal(cache.size, 0, 'Consumed ad must be released');
  assert.equal((await adCall(5, 'showAsync', 'missing')).status, 400, 'Missing ad must never grant a reward');
  context.FBInstant.getRewardedVideoAsync = async () => ({ loadAsync: async () => {}, showAsync: async () => { throw new Error('USER_INPUT'); } });
  result = await call(6, 'getRewardedVideoAsync', ['reward']);
  ad = JSON.parse(result.result);
  assert.equal((await adCall(7, 'showAsync', ad.$1.adInstanceID)).status, 400, 'Cancelled video must reject');
  assert.equal(cache.size, 0);
  context.FBInstant.getRewardedVideoAsync = async () => { throw new Error('NO_FILL'); };
  assert.equal((await call(8, 'getRewardedVideoAsync', ['reward'])).status, 400);
  context.FBInstant.getRewardedVideoAsync = () => { throw new Error('Synchronous failure'); };
  assert.equal((await call(9, 'getRewardedVideoAsync', ['reward'])).status, 400);
  console.log('PASS: missing APIs, public ad objects, load/show, cancellation, NO_FILL, synchronous failure, cache cleanup');
})().catch(error => { console.error(error); process.exitCode = 1; });
