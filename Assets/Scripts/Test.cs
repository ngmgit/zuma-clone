using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using DG.Tweening;

public class Test : MonoBehaviour
{
   public Transform ObjectToMove;
   private float distance;
   public BGCurve curve;
   public float tweenSpeed;
   public Transform followTransform;
   public Ease easeSetting;

   /// <summary>
   /// Start is called on the frame when a script is enabled just before
   /// any of the Update methods is called the first time.
   /// </summary>
   private void Start()
   {
       //curve = GetComponent<BGCurve>();
       //Debug.Log(curve.Points.Length);

   }

   void Update()
   {
        MoveTween();
   }

   private void MoveOnCurve()
   {
       //increase distance
       distance += 5 * Time.deltaTime;

       //calculate position and tangent
       Vector3 tangent;
       ObjectToMove.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(distance, out tangent);
       ObjectToMove.rotation = Quaternion.LookRotation(tangent);
   }

   private void MoveTween()
   {
       ObjectToMove.transform
        .DOMove(followTransform.transform.position, tweenSpeed)
        .SetEase(Ease.OutBounce);
   }
}
