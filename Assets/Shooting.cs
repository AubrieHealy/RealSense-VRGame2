using UnityEngine;
using System.Collections;
using System;

namespace RSUnityToolkit
{



    public class Shooting : MonoBehaviour
    {

        private PXCMSenseManager session = null;
        private pxcmStatus sts; // sts for debug log
        private pxcmStatus sts2; 
        private PXCMHandModule handAnalyzer;
        private PXCMFaceModule faceAnalyzer; 

        // Use this for initialization
        void Start()
        {
            // Creates an instance of the sense manager to be called later
            session = PXCMSenseManager.CreateInstance();


            //Output an error if there is no instance of the sense manager 
            if (session == null)
            {
                Debug.LogError("SenseManager Init Failed!");
            }


            // Enables hand tracking
            sts = session.EnableHand();
            handAnalyzer = session.QueryHand();

            sts2 = session.EnableFace();
            faceAnalyzer = session.QueryFace();  


            if (sts != pxcmStatus.PXCM_STATUS_NO_ERROR)
                Debug.LogError("PXCSenseManager.EnableHand: " + sts);


            if (sts2 != pxcmStatus.PXCM_STATUS_NO_ERROR)
                Debug.LogError("PXCSenseManager.EnableFace: " + sts2);



            // Creates the session 
            sts = session.Init();

            sts2 = session.Init(); 


            if (sts != pxcmStatus.PXCM_STATUS_NO_ERROR)
                Debug.LogError("PXCSenseManager.Init: " + sts);


            if (sts2 != pxcmStatus.PXCM_STATUS_NO_ERROR)
                Debug.LogError("PXCSenseManager.Init: " + sts2);



            // Creates a hand config for future data 
            PXCMHandConfiguration handconfig = handAnalyzer.CreateActiveConfiguration();

            PXCMFaceConfiguration faceconfig = faceAnalyzer.CreateActiveConfiguration(); 


            //If there is handconfig instance
            if (handconfig != null)
            {
                handconfig.EnableAllAlerts();
                handconfig.ApplyChanges();
                handconfig.Dispose();
            }
            if (faceconfig != null)
            {
                faceconfig.EnableAllAlerts();
                faceconfig.ApplyChanges();
                faceconfig.Dispose(); 
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (session == null)
                return;

            // For accessing hand data
            handAnalyzer = session.QueryHand();
            faceAnalyzer = session.QueryFace(); 


            if (handAnalyzer != null)
            {
                PXCMHandData handData = handAnalyzer.CreateOutput();
                if (handData != null)
                {
                    handData.Update();

                    PXCMHandData.IHand IHAND; // Ihand instance for accessing future data
                 //   Int32 IhandData; // for QueryOpenness Value
                //    PXCMPoint3DF32 location; // Stores hand tracking position 

                    //Fills IHAND with information to later be grabbed and used for tracking + openness 
                    handData.QueryHandData(PXCMHandData.AccessOrderType.ACCESS_ORDER_NEAR_TO_FAR, 0, out IHAND);


                    // If there is data in Ihand
                    if (IHAND != null)
                    {

                        // Debug.DrawLine(transform.position, hit.point, Color.red);


                        // Inits hand tracking from the center of the hand. 
                //        location = IHAND.QueryMassCenterWorld();
                  //      if (mCurrentDart != null)
                    //    {
                      //      Vector3 locationUnity = new Vector3(location.x, location.y, location.z);
                      //      mCurrentDart.transform.localPosition = locationUnity * RSScale;
                       // }

                    }

                }
                handAnalyzer.Dispose();
                session.ReleaseFrame();

            }


            if (faceAnalyzer != null)
            {
              
                PXCMFaceData facedata = faceAnalyzer.CreateOutput();
                if (facedata != null)
                {
                    Int32 nfaces = facedata.QueryNumberOfDetectedFaces();
                    for (Int32 i = 0; i < nfaces; i++)
                    {

                        // Retrieve the face landmark data instance

                        PXCMFaceData.Face face = facedata.QueryFaceByIndex(i);

                        PXCMFaceData.PoseData pdata = face.QueryPose();



                        // retrieve the pose information

                        PXCMFaceData.PoseEulerAngles angles;

                        pdata.QueryPoseAngles(out angles);
                        Debug.Log("Eular Angles yaw : " + angles.yaw);
                        Debug.Log("Eular Angles pitch: " + angles.pitch);
                        Debug.Log("Eular Angles Roll: " + angles.roll);
                        angles.pitch = gameObject.transform.rotation.z;
                        angles.yaw = gameObject.transform.rotation.y; 

                    }

                    // device is a PXCMCapture.Device instance

                 

                }
            }


        }
    }
}
