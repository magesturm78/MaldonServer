using System;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class DoorScripts
    {
        public static void Initialize()
        {
            MapScriptManager.Register("door1", ProcessDoor1);
            MapScriptManager.Register("delaydoor1", ProcessDoor1);

            MapScriptManager.Register("door3", ProcessDoor3);
            MapScriptManager.Register("delaydoor3", ProcessDoor3);

            MapScriptManager.Register("door7", ProcessDoor7);
            MapScriptManager.Register("delaydoor7", ProcessDoor7);

            MapScriptManager.Register("door9", ProcessDoor9);
            MapScriptManager.Register("delaydoor9", ProcessDoor9);

            MapScriptManager.Register("doorvalour1", ProcessDoorValour1);
            MapScriptManager.Register("doorvalour2", ProcessDoorValour2);
            MapScriptManager.Register("doorvalour3", ProcessDoorValour3);
            MapScriptManager.Register("doorvalour4", ProcessDoorValour4);
            MapScriptManager.Register("doorvalour5", ProcessDoorValour5);
            MapScriptManager.Register("doorvalour6", ProcessDoorValour6);
            MapScriptManager.Register("doorvalour7", ProcessDoorValour7);
            MapScriptManager.Register("doorvalour8", ProcessDoorValour8);
        }

        public static void ProcessDoor1(IMobile mobile)
        {
            Point3D doorLocation = Utility.GetLocation(mobile.Location, Direction.West);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoor3(IMobile mobile)
        {
            Point3D doorLocation = Utility.GetLocation(mobile.Location, Direction.South);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoor7(IMobile mobile)
        {
            Point3D doorLocation = Utility.GetLocation(mobile.Location, Direction.North);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoor9(IMobile mobile)
        {
            Point3D doorLocation = Utility.GetLocation(mobile.Location, Direction.East);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoorValour1(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(315, 177, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(316, 177, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(317, 177, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoorValour2(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(277, 126, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(277, 127, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(277, 128, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoorValour3(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(387, 174, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(388, 174, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(389, 174, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoorValour4(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(406, 127, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(406, 128, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(406, 129, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoorValour5(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(406, 80, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(406, 81, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(406, 82, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoorValour6(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(371, 45, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(372, 45, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(373, 45, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }
        public static void ProcessDoorValour7(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(294, 45, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(295, 45, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(296, 45, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessDoorValour8(IMobile mobile)
        {
            Point3D doorLocation = new Point3D(277, 91, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(277, 92, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
            doorLocation = new Point3D(277, 93, 0);
            mobile.Map.OpenDoor(mobile, doorLocation);
        }

        public static void ProcessArena1(IMobile mobile)
        {
            Point3D doorLocation = mobile.Location;
            mobile.Map.OpenDoor(mobile, doorLocation);
            //mobile.Health -= 10;
        }
    }
}