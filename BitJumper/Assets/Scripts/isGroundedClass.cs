using UnityEngine;
public class isGroundedClass  : MonoBehaviour {
 private bool isGrounded = false;

 public bool IsGrounded {
    get {
        return isGrounded;
    }
 }
 void OnCollisionEnter(Collision col)
 {
     if(col.gameObject.tag == "Ground"){
         isGrounded = true;
     }
 }

  void OnCollisionStay(Collision col)
 {
     if(col.gameObject.tag == "Ground"){
         isGrounded = true;
     }
 }
 
 void OnCollisionExit(Collision col)
 {
     if(col.gameObject.tag == "Ground"){
         isGrounded = false;
     }
 }

}
