﻿using System.IO;
using UnityEngine;
using SensorNoise;

namespace RosSharp.RosBridgeClient
{
    public class ImuSensor : UnityPublisher<MessageTypes.Sensor.Imu>
    {
        public bool EnableNoise;
        public NoiseSetteing Setting = new NoiseSetteing();

        [System.Serializable]
        public class NoiseSetteing
        {
            public double Sigma = 0.1;
        }

        public string FrameId = "imu";
        private Transform trans;
        private Rigidbody rb;
        private MessageTypes.Sensor.Imu message;
        private float deltaTime;

        /// previous value
        private Vector3 LastVelocity = Vector3.zero;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            rb = GetComponent<Rigidbody>();
            trans = GetComponent<Transform>();
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Sensor.Imu();
            message.header.frame_id = FrameId;
        }

        private void UpdateMessage()
        {
            message.header.Update();
            Transform transform = trans;
            Rigidbody rigidbody = rb;
            deltaTime = Time.deltaTime;
            message.orientation = GetGeometryQuaternion(transform);
            message.angular_velocity = GetAngularVelocity(rigidbody);
            message.linear_acceleration = GetLinearAcceleration(transform, rigidbody);

            if (EnableNoise)
            {
                var noise = new BoxMullerNoise();
                message.orientation.x = noise.next(message.orientation.x, Setting.Sigma);
                message.orientation.y = noise.next(message.orientation.y, Setting.Sigma);
                message.orientation.z = noise.next(message.orientation.z, Setting.Sigma);
                message.orientation.w = noise.next(message.orientation.w, Setting.Sigma);
                message.angular_velocity.x = noise.next(message.angular_velocity.x, Setting.Sigma);
                message.angular_velocity.y = noise.next(message.angular_velocity.y, Setting.Sigma);
                message.angular_velocity.z = noise.next(message.angular_velocity.z, Setting.Sigma);
                message.linear_acceleration.x = noise.next(message.linear_acceleration.x, Setting.Sigma);
                message.linear_acceleration.y = noise.next(message.linear_acceleration.y, Setting.Sigma);
                message.linear_acceleration.z = noise.next(message.linear_acceleration.z, Setting.Sigma);
            }

            Publish(message);
        }

        private MessageTypes.Geometry.Quaternion GetGeometryQuaternion(Transform transform)
        {
            MessageTypes.Geometry.Quaternion geometryQuaternion = new MessageTypes.Geometry.Quaternion();
            geometryQuaternion.x = transform.rotation.Unity2Ros().x;
            geometryQuaternion.y = transform.rotation.Unity2Ros().y;
            geometryQuaternion.z = transform.rotation.Unity2Ros().z;
            geometryQuaternion.w = transform.rotation.Unity2Ros().w;
            return geometryQuaternion;
        }

        private MessageTypes.Geometry.Vector3 GetAngularVelocity(Rigidbody rigidbody)
        {
            MessageTypes.Geometry.Vector3 result = new MessageTypes.Geometry.Vector3();
            result.x = rigidbody.angularVelocity.Unity2Ros().x;
            result.y = rigidbody.angularVelocity.Unity2Ros().y;
            result.z = rigidbody.angularVelocity.Unity2Ros().z;
            return result;
        }

        private MessageTypes.Geometry.Vector3 GetLinearAcceleration(Transform transform, Rigidbody rigidbody)
        {
            // move element
            Vector3 localLinearVelocity = transform.InverseTransformDirection(rigidbody.velocity);
            Vector3 acceleration = ( localLinearVelocity - LastVelocity ) / deltaTime;
            LastVelocity = localLinearVelocity;
            //gravity element
            acceleration += transform.InverseTransformDirection(Physics.gravity);

            MessageTypes.Geometry.Vector3 result = new MessageTypes.Geometry.Vector3();
            result.x = acceleration.Unity2Ros().x;
            result.y = acceleration.Unity2Ros().y;
            result.z = acceleration.Unity2Ros().z;
            return result;
        }
    }
}
