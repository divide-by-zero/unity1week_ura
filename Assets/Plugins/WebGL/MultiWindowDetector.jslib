var MultiWindowDetectorPlugin = {

    $mwd: {
        channel: null,
        tabId: null,
        gameObjectName: null
    },

    MWD_Init: function (gameObjectNamePtr) {
        mwd.gameObjectName = UTF8ToString(gameObjectNamePtr);
        mwd.tabId = Date.now().toString(36) + Math.random().toString(36).substr(2, 5);

        try {
            mwd.channel = new BroadcastChannel('unity_multiwindow');
        } catch (e) {
            return;
        }

        mwd.channel.onmessage = function (ev) {
            var data = ev.data;
            if (!data || data.senderId === mwd.tabId) return;

            if (data.type === 'ping') {
                SendMessage(mwd.gameObjectName, 'OnPingReceived', data.message || '');
            } else if (data.type === 'pong') {
                SendMessage(mwd.gameObjectName, 'OnPongReceived', data.message || '');
            }
        };
    },

    MWD_Ping: function (messagePtr) {
        if (!mwd.channel) return;
        mwd.channel.postMessage({ type: 'ping', senderId: mwd.tabId, message: UTF8ToString(messagePtr) });
    },

    MWD_Pong: function (messagePtr) {
        if (!mwd.channel) return;
        mwd.channel.postMessage({ type: 'pong', senderId: mwd.tabId, message: UTF8ToString(messagePtr) });
    }
};

autoAddDeps(MultiWindowDetectorPlugin, '$mwd');
mergeInto(LibraryManager.library, MultiWindowDetectorPlugin);