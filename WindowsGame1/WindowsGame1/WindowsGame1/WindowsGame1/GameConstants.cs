using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    static class GameConstants
    {
        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 140f;
        public const float PlayfieldSizeZ = 100f;
        //Dalek constants
        public const int NumDaleks = 23;
        public const float DalekMinSpeed = 10.0f;
        public const float DalekMaxSpeed = 12.0f;
        public const float DalekSpeedAdjustment = 2.5f;
        public const float DalekScalar = 0.01f;

        //Reset lives
        public const int NumLives = 3;
        public const float lifeMinSpeed = 10.0f;
        public const float lifeMaxSpeed = 10.0f;
        public const float lifeSpeedAdjustment = 2.5f;
        public const float lifeScalar = 0.01f;

        //collision constants
        public const float DalekBoundingSphereScale = 0.525f;  //50% size
        public const float ShipBoundingSphereScale = 0.5f;  //100% size
        public const float LaserBoundingSphereScale = 0.015f;  //50% size
        public const float cherryBoundingSphereScale = 0.7f;
        //bullet constants
        public const int NumLasers = 300;
        public const float LaserSpeedAdjustment = 1.0f;
        public const float LaserScalar = 0.002f;

    }
}
