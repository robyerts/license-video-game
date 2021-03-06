﻿using UnityEngine;
using System.Collections;

namespace DigitalRuby.PyroParticles
{
    public interface ICollisionHandler
    {
        void HandleCollision(GameObject obj, Collision c);
    }

    /// <summary>
    /// This script simply allows forwarding collision events for the objects that collide with something. This
    /// allows you to have a generic collision handler and attach a collision forwarder to your child objects.
    /// In addition, you also get access to the game object that is colliding, along with the object being
    /// collided into, which is helpful.
    /// </summary>
    public class FireCollisionForwardScript : MonoBehaviour
    {
        public ICollisionHandler CollisionHandler;

        public void OnCollisionEnter(Collision col)
        {
            Debug.Log(col.gameObject.tag + " - inside FireCollisionForwardScript OnCollisionEnter");
            Debug.Log("you can put the collision logic inside this firebolt collider script");
            CollisionHandler.HandleCollision(gameObject, col);
            if(col.gameObject.tag == "Player")
            {

            }
        }
    }
}
