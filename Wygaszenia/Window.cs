using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wygaszenia
{
    public class Window : WindowSupport
    {
        Draw mDraw;



        public Window(Panel _panelViewport)
        {
            panelViewport = _panelViewport;

            mDraw = new Draw();

            service = new GraphicsDeviceService(panelViewport.Handle, panelViewport.Width, panelViewport.Height);
            service.DeviceResetting += mWinForm_DeviceResetting;
            service.DeviceReset += mWinForm_DeviceReset;

            services = new ServiceContainer();
            services.AddService<IGraphicsDeviceService>(service);
            content = new ContentManager(services, "Content");



            mCamera = new Camera();
            mCamera.Initialize();

            DeviceReset();
            mDraw.SizeChanged(panelViewport, service.GraphicsDevice, service, spriteBatch, spriteFont, mSimpleEffect, mCamera, moon);

        }

        public void perspective(bool state)
        {
            mCamera.perspective = state;
            Update();
            DeviceReset();
            mDraw.SizeChanged(panelViewport, service.GraphicsDevice, service, spriteBatch, spriteFont, mSimpleEffect, mCamera, moon);
        }

        public void Background(Color col)
        {
            mDraw.BackgroundCol.R = col.R;
            mDraw.BackgroundCol.G = col.G;
            mDraw.BackgroundCol.B = col.B;
        }
        public void Background(Microsoft.Xna.Framework.Color col)
        {
                mDraw.BackgroundCol = col;
        }

        public Microsoft.Xna.Framework.Color GetBackground()
        {
            return mDraw.BackgroundCol;
        }


        void mWinForm_DeviceReset(Object sender, EventArgs e)
        {
            DeviceReset();
            mDraw.SizeChanged(panelViewport, service.GraphicsDevice, service, spriteBatch, spriteFont, mSimpleEffect, mCamera, moon);
        }

        public void Update()//Flags flags
        {
            mDraw.UpdateScene(panelViewport);
        }

        //Start rander the scene
        public void Render()
        {
            if (resizing) return;
           

            if (this.service.GraphicsDevice != null)
            {
                if (panelViewport.Name == "panel1")
                    mDraw.Scene(panelViewport);
                if (panelViewport.Name == "panel6")
                    mDraw.SceneRec(panelViewport);
            }

            try
            {
                if (service.GraphicsDevice != null)
                    service.GraphicsDevice.Present();
            }
            catch (Exception ex)
            {
                service.ResetDevice(panelViewport.Width, panelViewport.Height);
                String str = "Plot error. " + ex.ToString();
                // MessageBox.Show(str);
                //  System.Windows.Forms.Application.Exit();
            }
        }

    }
}
