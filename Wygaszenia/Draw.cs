using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Wygaszenia
{
    public class Draw : GraphicsHelper
    {

        private GraphicsDeviceService service;
        private SpriteBatch spriteBatch = null;
        private SpriteFont spriteFont;
        private BasicEffect mSimpleEffect;
        //private GraphicsHelper graphisc = new GraphicsHelper();

        private Camera mCamera;
        Model moon;

        // bool zoomed;
        public Color BackgroundCol;

        Vector3 LightECI;
        Vector3 atomECI;


        public void SizeChanged(Panel panelViewport, GraphicsDevice graphicsDevice, GraphicsDeviceService _service, SpriteBatch _spriteBatch, SpriteFont _spriteFont, BasicEffect _mSimpleEffect, Camera _mcamera, Model _moon)
        {
            service = _service;
            spriteBatch = _spriteBatch;
            spriteFont = _spriteFont;
            mSimpleEffect = _mSimpleEffect;
            UpdateScene(panelViewport);
            mCamera = _mcamera;
            moon = _moon;
        }

        public void Scene(Panel panelViewport)
        {
            int i;
            Vector3 AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
            Vector3 SpecularColor = new Vector3(0.9f, 0.9f, 0.9f);
            Vector3 DirectionalLight0_DiffuseColor = new Vector3(.5f, .5f, .5f);
            Color col = new Color(250, 250, 250);
            Color black = new Color(0, 0, 0, 200);

            service.GraphicsDevice.Clear(BackgroundCol);

            LightECI = new Vector3(-100, -90, -90);
            Vector3 VLight;
            VLight = Vector3.Transform(-LightECI, mCamera.Rotate);
            VLight = LightECI;// Vector3.Normalize(VLight);

            Matrix translation = Matrix.CreateTranslation(mCamera.ptTempPosition.X / 10, -mCamera.ptTempPosition.Y / 10, 0);
            Matrix ScaleRotateView = mCamera.Scale * mCamera.Rotate * translation * mCamera.View;
            service.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            #region atoms
            Order_Atoms3D(ScaleRotateView, Wygaszenia.AllAtoms);


            Matrix scale;

            if (mCamera.perspective)
                scale = Matrix.CreateScale(Flags.ballSize, Flags.ballSize, Flags.ballSize);
            else
                scale = Matrix.CreateScale(Flags.ballSize * 10, Flags.ballSize * 10, Flags.ballSize * 10);

            for (i = 0; i < Wygaszenia.AllAtoms.Count; i++)
            {
                Matrix world = Matrix.Identity;
                int Z_order = Wygaszenia.AllAtoms[i].Z_Depth;

                if (mCamera.perspective)
                    atomECI = Wygaszenia.AllAtoms[Z_order].position * 100;
                else
                    atomECI = Wygaszenia.AllAtoms[Z_order].position * 1000;

                AmbientLightColor = Wygaszenia.AllAtoms[Z_order].color;

                world *= Matrix.CreateTranslation(atomECI);
                world *= ScaleRotateView;



                Matrix[] transforms = new Matrix[moon.Bones.Count];
                moon.CopyAbsoluteBoneTransformsTo(transforms);


                foreach (ModelMesh mesh in moon.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.PreferPerPixelLighting = true;
                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = AmbientLightColor;
                        effect.SpecularColor = SpecularColor;
                        effect.SpecularPower = 9;
                        effect.DirectionalLight0.Direction = VLight;
                        effect.DirectionalLight0.DiffuseColor = DirectionalLight0_DiffuseColor/100;
                        effect.DirectionalLight0.SpecularColor = SpecularColor;
                        // effect.View = mCamera.View;
                        //effect.World = world;
                        effect.Projection = mCamera.Projection;
                        effect.World = transforms[mesh.ParentBone.Index] * scale * world;
                    }
                    mesh.Draw();
                }
            }
            #endregion


            #region Cell
            Vector3[] cellFrame = new Vector3[8];
            Vector3 tmp;
            Color col_frame = new Color(220, 220, 220, 0);
            i = 0;
            foreach (object3d element in Wygaszenia.UnitCellFrame)
            {
                cellFrame[i] = new Vector3();

                if (mCamera.perspective)
                    tmp = element.position * 100;
                else
                    tmp = element.position * 1000;

                cellFrame[i] = Vector3.Transform(tmp, ScaleRotateView);//*mCamera.Projection
                i++;
            }

            Line(service, mSimpleEffect, cellFrame[0], cellFrame[1], new Color(250, 0, 0, 0));
            Line(service, mSimpleEffect, cellFrame[1], cellFrame[2], col_frame);
            Line(service, mSimpleEffect, cellFrame[2], cellFrame[3], col_frame);
            Line(service, mSimpleEffect, cellFrame[3], cellFrame[0], new Color(0, 250, 0, 0));

            Line(service, mSimpleEffect, cellFrame[4], cellFrame[5], col_frame);
            Line(service, mSimpleEffect, cellFrame[5], cellFrame[6], col_frame);
            Line(service, mSimpleEffect, cellFrame[6], cellFrame[7], col_frame);
            Line(service, mSimpleEffect, cellFrame[7], cellFrame[4], col_frame);

            Line(service, mSimpleEffect, cellFrame[0], cellFrame[4], new Color(0, 0, 250, 0));
            Line(service, mSimpleEffect, cellFrame[3], cellFrame[7], col_frame);

            Line(service, mSimpleEffect, cellFrame[1], cellFrame[5], col_frame);
            Line(service, mSimpleEffect, cellFrame[2], cellFrame[6], col_frame);

            FiledRectangle(service, mSimpleEffect, cellFrame[0], cellFrame[1], cellFrame[2], new Color(122, 11, 55, 11));
            FiledRectangle(service, mSimpleEffect, cellFrame[0], cellFrame[2], cellFrame[3], new Color(122, 11, 55, 11));
            #endregion

            int width2 = panelViewport.Width / 2;
            int height2 = panelViewport.Height / 2;
            spriteBatch.Begin();

            if (mCamera.perspective)
            {
                spriteBatch.DrawString(spriteFont, "a", new Vector2(cellFrame[1].X * 8 + width2, -cellFrame[1].Y * 8 + height2), new Color(250, 0, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "b", new Vector2(cellFrame[3].X * 8 + width2, -cellFrame[3].Y * 8 + height2), new Color(0, 250, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "c", new Vector2(cellFrame[4].X * 8 + width2, -cellFrame[4].Y * 8 + height2), new Color(0, 0, 250, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.DrawString(spriteFont, "a", new Vector2(cellFrame[1].X + width2, -cellFrame[1].Y + height2), new Color(250, 0, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "b", new Vector2(cellFrame[3].X + width2, -cellFrame[3].Y + height2), new Color(0, 250, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "c", new Vector2(cellFrame[4].X + width2, -cellFrame[4].Y + height2), new Color(0, 0, 250, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
            }


            //spriteBatch.DrawString(spriteFont, (2 / 1000000).ToString("0.00") + " MSPS  Radio " + 1, new Vector2(panelViewport.Width - 130, 1), white, 0, new Vector2(0, 0), 0.27f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public void SceneRec(Panel panelViewport)
        {
            int i;
            Vector3 AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
            Vector3 SpecularColor = new Vector3(0.9f, 0.9f, 0.9f);
            Vector3 DirectionalLight0_DiffuseColor = new Vector3(.5f, .5f, .5f);
            Color col = new Color(250, 250, 250);
            Color black = new Color(0, 0, 0, 200);

            service.GraphicsDevice.Clear(BackgroundCol);

            LightECI = new Vector3(-100, -100, -100);
            Vector3 VLight;
            //  VLight = Vector3.Transform(LightECI, -mCamera.Rotate);
            VLight = LightECI;// Vector3.Normalize(LightECI);

            Matrix translation = Matrix.CreateTranslation(mCamera.ptTempPosition.X / 10, -mCamera.ptTempPosition.Y / 10, 0);
            Matrix ScaleRotateView = mCamera.Scale * mCamera.Rotate * translation * mCamera.View;
            service.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            #region reflections
            Order_Atoms3D(ScaleRotateView, Wygaszenia.rec_points);


  
            for (i = 0; i < Wygaszenia.rec_points.Count; i++)
            {
                Matrix world = Matrix.Identity;
                int Z_order = Wygaszenia.rec_points[i].Z_Depth;


                Matrix scale;
                float size = Flags.ballSizeRec * Wygaszenia.rec_points[Z_order].F;
                if (mCamera.perspective)
                    scale = Matrix.CreateScale(size, size, size);
                else
                    scale = Matrix.CreateScale(size * 10, size * 10, size * 10);



                if (mCamera.perspective)
                    atomECI = Wygaszenia.rec_points[Z_order].position * 100;
                else
                    atomECI = Wygaszenia.rec_points[Z_order].position * 1000;

                //colors
               AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
               SpecularColor = new Vector3(0.9f, 0.9f, 0.9f);

                if (Wygaszenia.rec_points[Z_order].l==0)
                    AmbientLightColor = new Vector3(0.9f, 0.1f, 0.1f);

                if (Wygaszenia.rec_points[Z_order].l == 1)
                    AmbientLightColor =  new Vector3(0.9f, 0.9f, 0.1f);

                world *= Matrix.CreateTranslation(atomECI);
                world *= ScaleRotateView;


                Matrix[] transforms = new Matrix[moon.Bones.Count];
                moon.CopyAbsoluteBoneTransformsTo(transforms);


                foreach (ModelMesh mesh in moon.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.PreferPerPixelLighting = true;
                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = AmbientLightColor;
                        effect.SpecularColor = SpecularColor;
                        effect.SpecularPower = 9;
                        effect.DirectionalLight0.Enabled=true;
                        effect.DirectionalLight0.Direction = VLight;
                        effect.DirectionalLight0.DiffuseColor = DirectionalLight0_DiffuseColor/100;
                        effect.DirectionalLight0.SpecularColor = SpecularColor;
                        // effect.View = mCamera.View;
                        //effect.World = world;
                        effect.Projection = mCamera.Projection;
                        effect.World = transforms[mesh.ParentBone.Index] * scale * world;
                    }
                    mesh.Draw();
                }
            }
            #endregion


            #region Cell
            Vector3[] cellFrame = new Vector3[8];
            Vector3 tmp;
            Color col_frame = new Color(220, 220, 220, 0);
            i = 0;
            foreach (object3d element in Wygaszenia.RecCellFrame)
            {
                cellFrame[i] = new Vector3();

                if (mCamera.perspective)
                    tmp = element.position * 100;
                else
                    tmp = element.position * 1000;

                cellFrame[i] = Vector3.Transform(tmp, ScaleRotateView);//*mCamera.Projection
                i++;
            }

            Line(service, mSimpleEffect, cellFrame[0], cellFrame[1], new Color(250, 0, 0, 0));
            Line(service, mSimpleEffect, cellFrame[1], cellFrame[2], col_frame);
            Line(service, mSimpleEffect, cellFrame[2], cellFrame[3], col_frame);
            Line(service, mSimpleEffect, cellFrame[3], cellFrame[0], new Color(0, 250, 0, 0));

            Line(service, mSimpleEffect, cellFrame[4], cellFrame[5], col_frame);
            Line(service, mSimpleEffect, cellFrame[5], cellFrame[6], col_frame);
            Line(service, mSimpleEffect, cellFrame[6], cellFrame[7], col_frame);
            Line(service, mSimpleEffect, cellFrame[7], cellFrame[4], col_frame);

            Line(service, mSimpleEffect, cellFrame[0], cellFrame[4], new Color(0, 0, 250, 0));
            Line(service, mSimpleEffect, cellFrame[3], cellFrame[7], col_frame);

            Line(service, mSimpleEffect, cellFrame[1], cellFrame[5], col_frame);
            Line(service, mSimpleEffect, cellFrame[2], cellFrame[6], col_frame);

            FiledRectangle(service, mSimpleEffect, cellFrame[0], cellFrame[1], cellFrame[2], new Color(122, 11, 55, 11));
            FiledRectangle(service, mSimpleEffect, cellFrame[0], cellFrame[2], cellFrame[3], new Color(122, 11, 55, 11));
            #endregion

            int width2 = panelViewport.Width / 2;
            int height2 = panelViewport.Height / 2;
            spriteBatch.Begin();

            if (mCamera.perspective)
            {
                spriteBatch.DrawString(spriteFont, "a*", new Vector2(cellFrame[1].X * 8 + width2, -cellFrame[1].Y * 8 + height2), new Color(250, 0, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "b*", new Vector2(cellFrame[3].X * 8 + width2, -cellFrame[3].Y * 8 + height2), new Color(0, 250, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "c*", new Vector2(cellFrame[4].X * 8 + width2, -cellFrame[4].Y * 8 + height2), new Color(0, 0, 250, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.DrawString(spriteFont, "a*", new Vector2(cellFrame[1].X + width2, -cellFrame[1].Y + height2), new Color(250, 0, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "b*", new Vector2(cellFrame[3].X + width2, -cellFrame[3].Y + height2), new Color(0, 250, 0, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "c*", new Vector2(cellFrame[4].X + width2, -cellFrame[4].Y + height2), new Color(0, 0, 250, 0), 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
            }


            //spriteBatch.DrawString(spriteFont, (2 / 1000000).ToString("0.00") + " MSPS  Radio " + 1, new Vector2(panelViewport.Width - 130, 1), white, 0, new Vector2(0, 0), 0.27f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        //--------------------------------------------------------------------------------------------------------------------------------
        void Order_Atoms3D(Matrix RotateViewnProjMatrix, List<object3d> ob3d)//
        {
            //--------------------------------------------------------------------------------------------------------------------------------
            Vector3 v_local;
            Vector3 v_real;
            float last_z;
            int iterate = 0;

            bool[] atomCheck = new bool[ob3d.Count];


            for (int m = 0; m < ob3d.Count; ++m)
            {
                last_z = 10000000.0f;
                for (int n = 0; n < ob3d.Count; ++n)
                {
                    v_local = ob3d[n].position * 100;
                    v_real = Vector3.Transform(v_local, RotateViewnProjMatrix);

                    if (!atomCheck[n] && v_real.Z<= last_z)
                    {
                        last_z = v_real.Z;
                        iterate = n;
                    }
                }
                ob3d[m].Z_Depth = iterate;
                atomCheck[iterate] = true;
            }         
        }




        public void UpdateScene(Panel panelViewport)
        {
            if (mCamera != null)
                mCamera.ApplyEffects(panelViewport);
        }

    }
}
