using UnityEngine;

namespace Trykli.Utilities
{
    /// <summary>
    /// Small compatibility layer over Rigidbody2D APIs that were renamed in Unity 6
    /// (velocity -> linearVelocity, drag -> linearDamping...). Gameplay code only uses these helpers
    /// so the project compiles without warnings on Unity 6 and older LTS versions.
    /// </summary>
    public static class PhysicsCompat
    {
        public static Vector2 GetVelocity(this Rigidbody2D body)
        {
#if UNITY_6000_0_OR_NEWER
            return body.linearVelocity;
#else
            return body.velocity;
#endif
        }

        public static void SetVelocity(this Rigidbody2D body, Vector2 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            body.linearVelocity = velocity;
#else
            body.velocity = velocity;
#endif
        }

        public static float GetLinearDamping(this Rigidbody2D body)
        {
#if UNITY_6000_0_OR_NEWER
            return body.linearDamping;
#else
            return body.drag;
#endif
        }

        public static void SetLinearDamping(this Rigidbody2D body, float damping)
        {
#if UNITY_6000_0_OR_NEWER
            body.linearDamping = damping;
#else
            body.drag = damping;
#endif
        }

        public static void SetAngularDamping(this Rigidbody2D body, float damping)
        {
#if UNITY_6000_0_OR_NEWER
            body.angularDamping = damping;
#else
            body.angularDrag = damping;
#endif
        }

        /// <summary>Configures a body that is moved by script (moving / rotating platforms).</summary>
        public static void MakeKinematic(this Rigidbody2D body)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.useFullKinematicContacts = true;
            body.interpolation = RigidbodyInterpolation2D.None;
        }
    }
}
