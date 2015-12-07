using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace ViewPortOpenGL
{
    public class ViewPortWindow
    {
        public Vector3[] pointsArray;
        public bool[] stimulusActive;

        public  ViewPortWindow()
        {
            initPoints();
        }

        public void Run()
        {
            //initPoints();
            Color c = Color.MidnightBlue;
            Matrix4 m = Matrix4.Identity;
            float pos = 0;
            float rot = 0;

            using (var game = new GameWindow())
            {
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    // add game logic, input handling
                    if (game.Mouse[MouseButton.Left])
                    {
                        m = Matrix4.Identity;
                    }
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }
                    if (game.Keyboard[Key.W])
                    {
                        m=Matrix4.Mult(m,Matrix4.CreateTranslation(0, 0.1f, 0));
                        //pointsArray[0].Z = pointsArray[0].Z + 0.1f;
                    }
                    if (game.Keyboard[Key.S])
                    {
                        m=Matrix4.Mult(m,Matrix4.CreateTranslation(0, -0.1f, 0));
                        //pointsArray[0].Z = pointsArray[0].Z - 0.1f;
                    }
                    if (game.Keyboard[Key.A])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateTranslation(-0.1f, 0, 0));
                        //pointsArray[0].X = pointsArray[0].X - 0.1f;
                    }
                    if (game.Keyboard[Key.D])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateTranslation(+0.1f, 0, 0));
                        //pointsArray[0].X = pointsArray[0].X + 0.1f;
                    }


                    if (game.Keyboard[Key.Up])
                    {
                        //rot += .1f;
                        //m= Matrix4.CreateTranslation(pos, 0, 0);
                        m = Matrix4.Mult(m, Matrix4.CreateRotationX(.1f));
                    }
                    if (game.Keyboard[Key.Right])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateRotationY(.1f));
                    }
                    if (game.Keyboard[Key.Left])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateRotationY(-0.1f));
                    }
                    if (game.Keyboard[Key.Down])
                    {

                        m = Matrix4.Mult(m, Matrix4.CreateRotationX(-0.1f));
                    }
                };

                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadMatrix(ref m);
                    //GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(Color.White);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(1f, 0f, 0f);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(0f, 1f, 0f);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(0f, 0f, 1f);
                    GL.End();


                    GL.Begin(PrimitiveType.Points);
                    GL.Color3(Color.Red);
                    for (int i = 0; i < pointsArray.Length; i++)
                    {
                        if (stimulusActive[i])
                            GL.Color3(Color.Red);
                        else
                            GL.Color3(Color.Cyan);

                        if (i == pointsArray.Length - 1)
                            GL.Color3(Color.White);

                        GL.Vertex3(pointsArray[i]);
                    }
                    //GL.Vertex3(0.5f, 0.5, 0.5);
                    GL.End();
                    /*
                    GL.Begin(PrimitiveType.Triangles);

                    GL.Color3(c);
                    GL.Vertex2(-0.5f, 0.5f);
                    GL.Color3(Color.SpringGreen);
                    GL.Vertex2(0.0f, -0.5f);
                    GL.Color3(Color.Ivory);
                    GL.Vertex2(0.5f, 0.5f);

                    GL.End();
                    
                    GL.Begin(PrimitiveType.Triangles);

                    GL.Color3(c);
                    GL.Vertex2(-0.5f, 0.5f);
                    GL.Color3(Color.SpringGreen);
                    GL.Vertex2(0.0f, 1.0f);
                    GL.Color3(Color.Red);
                    GL.Vertex2(-1.0f, 1.0f);

                    GL.End();*/


                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }

        }

        public  void initPoints()
        {
            Array.Resize(ref pointsArray, 1);
            pointsArray[0] = new Vector3(0.0f, 0.0f, 0.0f);

            Array.Resize(ref this.stimulusActive, 1);
            stimulusActive[0] = false;

        }

        public  void addPoints(float x, float y, float z,bool stimulusActive)
        {
            Array.Resize(ref pointsArray, pointsArray.Length + 1);
            Array.Resize(ref this.stimulusActive, this.stimulusActive.Length + 1);
            pointsArray[pointsArray.Length - 1] = new Vector3(x, y, z);
            this.stimulusActive[this.stimulusActive.Length - 1] = stimulusActive;

        }
    }
}
