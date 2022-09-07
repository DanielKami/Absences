using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wygaszenia
{
    public class Camera
    {
        public bool perspective;
        //Matrices
        public Matrix View;
        public Matrix Projection;
        public Matrix Rotate;
        public Matrix Rotate2;
        public Matrix RotateZ;
        public Matrix Scale;

        public float zoom;
        public float OneOverZoom;
        public bool flag_zooming;
        public bool flag_moving;
        public bool flag_rotating;

        public Vector3 g_cameraPos;//EyePt
        public Vector3 g_targetPos;
        public Vector3 g_system_translate;

        //TouchPanel inputs
        public Vector2 ptMouseCurrent;
        public Vector2 ptMousePrevious;
        public Vector2 ptMouseDoubleTap;
        public Vector2 ptMousePosition;
        public Vector2 ptMouseZoomDrag;
        public Vector2 ptTempPosition;

        public float ImgZoomActualWidth;
        public float ImgZoomActualHeight;

        private float g_yow;
        public float g_pitch;
        private float g_arcX, g_arcY;

        private Vector3 CameraRotateAroundX;			// When the orientation of camera change the z direction for arcball too
        private Vector3 CameraRotateAroundY;			// to follow these changes we have to change 
        private Vector3 CameraRotateAroundZ;			// the rotation directions to be z always pointing inside the screen

        //Camera parameters for 3D
        float ROTATION_STEP = 0.015f;
        float ROTATION_STEP_Z = 0.006f;
        float SHIFT_STEP = 1;

        float m_fCameraYawAngle;		//Yaw to follow the position of camera
        float m_fCameraPitchAngle;		//Pich to follow the position of camera

        //Quaternions
        //       private Quaternion MainQuad;
        //       int Quaternion_normalisation;
        //---------------------------------------------------------------

        public void Initialize()
        {

            perspective = false;
            zoom =10;
            OneOverZoom = 1.0f / zoom;

            //    Quaternion_normalisation = 0;
            g_cameraPos = new Vector3(0, 0, 80);
            g_targetPos = new Vector3(0, 0, 0);

            m_fCameraYawAngle = 0.0f;		//Yaw to follow the position of camera
            m_fCameraPitchAngle = 0.0f;		//Pich to follow the position of camera
            //Initialize the main quaternion for the orientation matrix calculation

            //Does't work with quaternion match something wrong in XNA
            //MainQuad = new Quaternion(1, 0, 0, 0);
            Rotate = Matrix.Identity;
            Rotate2 = Matrix.Identity;
            RotateZ = Matrix.Identity;
            //Translation
            g_system_translate = new Vector3(0, 0, 0);

            flag_rotating = false;
            flag_zooming = false;
            flag_moving = false;

            ptMouseDoubleTap = Vector2.Zero;
            ptMousePosition = Vector2.Zero;
            ptMouseZoomDrag = Vector2.Zero;
            ptTempPosition = Vector2.Zero;
            ImgZoomActualWidth = 10000;
            ImgZoomActualHeight = 10000;    
        }



        public void scroll(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta < 0)
                zoom *= 1.1f;
            else
                zoom *= 0.9f;

            OneOverZoom = 1.0f / zoom;
        }

        public void MouseDown(MouseEventArgs e)
        {
            ptMousePrevious.X = e.X;
            ptMousePrevious.Y = e.Y;

            if (e.Button == MouseButtons.Left)
                flag_rotating = true;
            else
                flag_rotating = false;

            if (e.Button == MouseButtons.Right)
                flag_moving = true;
            else
                flag_moving = false;
        }

        public void MouseMove(MouseEventArgs e, Panel panelViewport)
        {

                if (e.Button == MouseButtons.Left)
                flag_rotating = true;
            else
                flag_rotating = false;

            if (e.Button == MouseButtons.Right)
                flag_moving = true;
            else
                flag_moving = false;

            ptMouseCurrent.X = e.X;
            ptMouseCurrent.Y = e.Y;
            //        
            //Moving
            if (flag_moving && !flag_zooming)
            {
                ptTempPosition -= SHIFT_STEP * (ptMousePrevious - ptMouseCurrent);
                ptMousePrevious = ptMouseCurrent;
            }

            //Rotating
            if (!flag_moving && !flag_zooming && flag_rotating)
            {
                g_yow = ROTATION_STEP * (ptMousePrevious.X - ptMouseCurrent.X);
                g_pitch = ROTATION_STEP * (ptMousePrevious.Y - ptMouseCurrent.Y);

                g_arcX = (panelViewport.Width) / 2 - ptMouseCurrent.X;
                g_arcY = (panelViewport.Height) / 2 - ptMouseCurrent.Y;
       
                ptMousePrevious = ptMouseCurrent;
              
            }
        }

        public void MouseUp(MouseEventArgs e)
        {
            flag_rotating = false;
            flag_moving = false;
            ptMouseCurrent = Vector2.Zero;
            g_pitch = 0;
            g_yow = 0;
        }

        //-----------------------------------------------------------------------------
        void UpdateCameraOrientationPichYow()
        {

            Matrix mCameraRot;
            mCameraRot = Matrix.CreateRotationX(m_fCameraYawAngle);
            mCameraRot = Matrix.CreateRotationY(m_fCameraPitchAngle);

            //axis vectors 
            Vector3 LocalRotateAroundX = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 LocalRotateAroundY = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 LocalRotateAroundZ = new Vector3(0.0f, 0.0f, 1.0f);

            Matrix InvView;
            //D3DXMatrixInverse( &InvView, NULL, &view );
            InvView = Matrix.Invert(View);
            // The axis basis vectors and camera position are stored inside the 
            // position matrix in the 4 rows of the camera's world matrix.
            // To figure out the yaw/pitch of the camera, we just need the Z basis vector
            // this is neccesary to follow by the arc ball the camera position in the word,
            // the basic rotators of quaternions are changed
            //D3DXVECTOR3* pZBasis = ( D3DXVECTOR3* )&InvView._31;
            Vector3 pZBasis;
            pZBasis.X = InvView.M31;
            pZBasis.Y = InvView.M32;
            pZBasis.Z = InvView.M33;

            m_fCameraYawAngle = (float)Math.Atan2(pZBasis.X, pZBasis.Z);
            m_fCameraPitchAngle = (float)(-Math.Atan2(pZBasis.Y, Math.Sqrt(pZBasis.Z * pZBasis.Z + pZBasis.X * pZBasis.X)));

            CameraRotateAroundX = Vector3.Transform(LocalRotateAroundX, mCameraRot);
            CameraRotateAroundY = Vector3.Transform(LocalRotateAroundY, mCameraRot);
            CameraRotateAroundZ = Vector3.Transform(LocalRotateAroundZ, mCameraRot);

            CameraRotateAroundX = LocalRotateAroundX;
            CameraRotateAroundY = LocalRotateAroundY;
            CameraRotateAroundZ = LocalRotateAroundZ;

        }

        void UpdateQuadRotation()
        {
            /*
                        //quaternions for unit rotations
                        Quaternion LocalRotQuadX = new Quaternion(1, 0, 0, 0);
                        Quaternion LocalRotQuadY = new Quaternion(1, 0, 0, 0);
                        Quaternion LocalRotQuadZ = new Quaternion(1, 0, 0, 0);
            */

            //Rotate around X,Y,Z of arcball
            //g_pitch added to rotate with different speed
            float roll_X = g_pitch * g_arcX;
            float roll_Y = g_yow * g_arcY;
            float roll = (float)ROTATION_STEP_Z * (roll_Y - roll_X);


            /*
            if (!FlagChangeRotationMode)
            {
                //Rotate around X axis
                LocalRotQuadX = Quaternion.CreateFromAxisAngle(CameraRotateAroundX, -g_yow);
                //Rotate around Y axis
                LocalRotQuadY = Quaternion.CreateFromAxisAngle(CameraRotateAroundY, g_pitch);
                //Rotate around Z axis
                LocalRotQuadZ = Quaternion.CreateFromAxisAngle(CameraRotateAroundZ, roll);
            }
            else
            {
                //Rotate around X axis
                LocalRotQuadX = Quaternion.CreateFromAxisAngle(CameraRotateAroundY, -g_yow);
                //Rotate around Y axis
                LocalRotQuadY = Quaternion.CreateFromAxisAngle(CameraRotateAroundX, g_pitch);
                //Rotate around Z axis
                LocalRotQuadZ = Quaternion.CreateFromAxisAngle(CameraRotateAroundZ, -roll);
            }
            */

            Matrix xtmp = Matrix.CreateFromAxisAngle(CameraRotateAroundY, -g_yow);
            Matrix ytmp = Matrix.CreateFromAxisAngle(CameraRotateAroundX, -g_pitch);
            Matrix ztmp = Matrix.CreateFromAxisAngle(CameraRotateAroundZ, roll);

            Rotate *= xtmp * ytmp * ztmp;

            /*
                        if (FlagChangeRotationMode)
                        {
                            //Rotate around X axis
                            LocalRotQuadX = Quaternion.CreateFromAxisAngle(CameraRotateAroundY, -g_yow);
                            //Rotate around Y axis
                            LocalRotQuadY = Quaternion.CreateFromAxisAngle(CameraRotateAroundX, g_pitch);
                            //Rotate around Z axis
                            LocalRotQuadZ = Quaternion.CreateFromAxisAngle(CameraRotateAroundZ, roll);
                            //build rotation matrix from quaternion
                            Rotate2 = Matrix.CreateFromQuaternion(MainQuad);
                        }

                        //Combine all rotations and update the main rotation quaternion
                        MainQuad *= LocalRotQuadX;
                        MainQuad *= LocalRotQuadY;
                        MainQuad *= LocalRotQuadZ;

                        Rotate = Matrix.CreateFromQuaternion(MainQuad);

            
                        //Check for Quaternion normalisation after N rotation events to recover it
                        Quaternion_normalisation++;

                        if (Quaternion_normalisation > 100)
                        {
                            Quaternion_normalisation = 0;
                            Quaternion.Normalize(MainQuad);
                        }
             */
        }

        ///-----------------------------------------------------------------------------
        /// Calculate all camera matrices. The rotation matrics is recovery every time from quaternion which stores the rotation state.
        ///-----------------------------------------------------------------------------
        public void ApplyEffects(System.Windows.Forms.Panel panelViewport)
        {
            float aspect, w= panelViewport.Width, h= panelViewport.Height;
           
                aspect = w / (h+1);
            
           if(perspective)
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspect, 0.1f, 20000000f);
           else
           Projection = Matrix.CreateOrthographic(w, h,-10000f, 10000.0f);
 

            Vector3 LookUp = new Vector3(0.0f, 1.0f, 0.0f);



            //if (g_cameraPos.X == 0 && g_cameraPos.Z == 0) LookUp = new Vector3(0.0f, 1.0f, 0.0f);

            View = Matrix.CreateLookAt(g_cameraPos, g_targetPos, LookUp);
            Scale = Matrix.CreateScale(OneOverZoom, OneOverZoom, OneOverZoom);

            UpdateCameraOrientationPichYow();
            UpdateQuadRotation();

            //build rotation matrix from quaternion
            //Rotate = Matrix.CreateFromQuaternion(MainQuad);


            //Matrix m_temp = Matrix.CreateTranslation(g_system_translate);
            //View *= m_temp;





        }

    }
}
