﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tile_Engine
{
    public class Camera
    {
        public float X
        {
            get { return _position.X; }
            set { _positionTranslation.M41 = -(_position.X = value); }
        }
        public float Y
        {
            get { return _position.Y; }
            set { _positionTranslation.M42 = -(_position.Y = value); }
        }
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _positionTranslation.M41 = -(_position.X = value.X);
                _positionTranslation.M42 = -(_position.Y = value.Y);
            }
        }
        public float Angle
        {
            get { return _angle; }
            set
            {
                _rotationZ.M22 = _rotationZ.M11 = (float)System.Math.Cos(_angle = value);
                _rotationZ.M21 = -(_rotationZ.M12 = (float)System.Math.Sin(_angle));
            }
        }
        public float Scale
        {
            get { return _scale.M11; }
            set { _scale.M11 = _scale.M22 = value; }
        }
        public Vector2 ScreenSize
        {
            get { return _screenSize; }
            set
            {
                _screenSize = value;
                _projection.M11 = (float)(2d / _screenSize.X);
                _projection.M22 = (float)(2d / -_screenSize.Y);
                _projection.M41 = (float)((double)_screenSize.X / -_screenSize.X);
                _projection.M42 = (float)((double)_screenSize.Y / _screenSize.Y);
                _screenTranslation.M41 = (_screenSize.X / 2f);
                _screenTranslation.M42 = (_screenSize.Y / 2f);
            }
        }

        public Matrix Transform { get; private set; }

        public Vector2 MousePosition { get { return _mousePosition; } }
        public Matrix Projection { get { return _projection; } }

        private Vector2 _position;
        private float _angle;
        private Vector2 _screenSize;
        private Matrix _positionTranslation;
        private Matrix _rotationZ;
        private Matrix _scale;
        private Matrix _screenTranslation;
        private Matrix _transformInvert;
        private Vector2 _mousePosition;
        private Matrix _projection;

        public Camera(float angle = 0, float scale = 1) : this(Vector2.Zero, angle, scale) { }
        public Camera(Vector2 position, float angle = 0, float scale = 1)
        {
            _positionTranslation = Matrix.CreateTranslation(-(_position.X = position.X), -(_position.Y = position.Y), 0);
            _rotationZ = Matrix.CreateRotationZ(-(_angle = angle));
            _scale = Matrix.CreateScale(scale, scale, 1);
            _screenSize = new Vector2(Game1.VirtualWidth, Game1.VirtualHeight);
            _screenTranslation = Matrix.CreateTranslation((_screenSize.X / 2f), (_screenSize.Y / 2f), 0);
            UpdateTransform();
            _projection = Matrix.CreateOrthographicOffCenter(0, _screenSize.X, _screenSize.Y, 0, 0, 1);
        }

        public void UpdateMousePosition(MouseState? mouseState = null)
        {
            if (!mouseState.HasValue)
                mouseState = Mouse.GetState();
            float mouseX = ((((float)mouseState.Value.Position.X / Game1.WindowWidth) * Game1.VirtualWidth) - Game1.ViewportX);
            float mouseY = ((((float)mouseState.Value.Position.Y / Game1.WindowHeight) * Game1.VirtualHeight) - Game1.ViewportY);
            _mousePosition.X = ((mouseX * _transformInvert.M11) + (mouseY * _transformInvert.M21) + _transformInvert.M41);
            _mousePosition.Y = ((mouseX * _transformInvert.M12) + (mouseY * _transformInvert.M22) + _transformInvert.M42);
        }

        public void UpdateTransform()
        {
            Transform = (_positionTranslation * _rotationZ * _scale * _screenTranslation);
            _transformInvert = Matrix.Invert(Transform);
        }
    }
}