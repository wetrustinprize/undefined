using UnityEngine;

namespace Undefined
{
    
    namespace AI
    {

        public class AIDetection : MonoBehaviour {

            ///<summary>Detects if is in a edge</summary>
            ///<param name="direction">The direction the AI is walking to</param>
            ///<param name="motor">The AI motor</param>
            ///<param name="self">A reference to the AI</param>
            public static bool Edge(Vector2 direction, Motor motor, GameObject self) {

                Vector2 pos = motor.GroundColliderPosition + (Vector2)self.transform.position;
                pos.x += motor.GroundCellingColliderSize.x * direction.x;

                foreach(Collider2D c in Physics2D.OverlapBoxAll(pos, motor.GroundCellingColliderSize, 0, motor.CollisionLayer))
                {
                    if(c == self.GetComponent<Collider2D>()) continue;
                    else
                    {
                        return false;
                    }
                }
                return true;

            }

            public static bool LimitedVision(float limitation, int lastDir, GameObject self, float maxDistance, LayerMask collisionLayer, GameObject target)
            {

                Vector2 direction = (self.transform.position - target.transform.position).normalized * -1;

                if(direction.x < limitation && lastDir == 1 || direction.x > -limitation && lastDir == -1)
                    return false;
                else
                    return Vision(self, maxDistance, collisionLayer, target);

            }


            public static bool Vision(GameObject self, float maxDistance, LayerMask collisionLayer, GameObject target)
            {

                Vector2 direction = (self.transform.position - target.transform.position).normalized * -1;
                UnityEngine.Debug.Log(direction);

                RaycastHit2D[] hit = Physics2D.RaycastAll((Vector2)self.transform.position, direction, maxDistance, collisionLayer);
                foreach(RaycastHit2D h in hit) {

                    if(h.collider.gameObject == self) continue;
                    if(h.collider.gameObject != target) break;
                    if(h.collider.gameObject == target) return true;
                }

                return false;

            }

            public class Debug {

                public static void Edge(Vector2 direction, Motor motor, GameObject self) {

                    Vector2 pos = motor.GroundColliderPosition + (Vector2)self.transform.position;
                    pos.x += motor.GroundCellingColliderSize.x * direction.x;

                    Gizmos.DrawCube(pos, motor.GroundCellingColliderSize);

                }

                public static void Vision(GameObject self, float maxDistance, GameObject target, bool limited = false, float limitation = 0f)
                {

                    Vector2 direction = (self.transform.position - target.transform.position).normalized * -1;
                    Vector2 limitedDirUp = new Vector2(limitation, 1);
                    Vector2 limitedDirDown = new Vector2(limitation, -1);

                    Gizmos.DrawRay(self.transform.position, direction);

                    if(!limited) return;

                    Gizmos.DrawRay(self.transform.position, limitedDirUp);
                    Gizmos.DrawRay(self.transform.position, limitedDirDown);
                    Gizmos.DrawRay(self.transform.position, -limitedDirUp);
                    Gizmos.DrawRay(self.transform.position, -limitedDirDown);

                }

            }

        }

    }

}
