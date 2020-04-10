using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public delegate void MapScriptFunction(IMobile mobile);

    public class MapScriptHandler
    {
        public string Script { get; private set; }
        public MapScriptFunction ProcessScript { get; private set; }

        public MapScriptHandler(string script, MapScriptFunction scriptFunction)
        {
            Script = script;
            ProcessScript = scriptFunction;
        }
    }

    public class MapScriptManager
    {
        public static Hashtable Handlers { get; private set; }

        public static void Configure()//Configure is run before initialization
        {
            Handlers = new Hashtable();
        }

        public static void Register(string name, MapScriptFunction scriptFunction)
        {
            Handlers[name] = new MapScriptHandler(name, scriptFunction);
        }

        public static MapScriptHandler GetScriptHandler(string name)
        {
            return (MapScriptHandler)Handlers[name];
        }
    }
}
