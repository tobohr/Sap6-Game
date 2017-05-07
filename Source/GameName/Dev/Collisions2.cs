namespace GameName.Dev {

//--------------------------------------
// USINGS
//--------------------------------------

using System;

using EngineName;
using EngineName.Components;
using EngineName.Components.Renderable;
using EngineName.Core;
using EngineName.Shaders;
using EngineName.Systems;
using EngineName.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//--------------------------------------
// CLASSES
//--------------------------------------

/// <summary>Provides a simple test case for collisions. Running it, you should see several spheres
///          colliding on the screen in a sane manner.</summary>
public sealed class Collisions2: Scene {
    //--------------------------------------
    // NON-PUBLIC FIELDS
    //--------------------------------------

    /// <summary>Used to create environment maps.</summary>
    private RenderingSystem mRenderer;

    //--------------------------------------
    // PUBLIC METHODS
    //--------------------------------------

    /// <summary>Initializes the scene.</summary>
    public override void Init() {
        AddSystems(new                    LogicSystem(),
                   new                  PhysicsSystem(),
                   new                   CameraSystem(),
                   mRenderer    = new RenderingSystem());

#if DEBUG
        AddSystem(new DebugOverlay());
#endif

        base.Init();

        InitCam();

        // Spawn a few balls.
        for (var i = 0; i < 3; i++) {
            var r = 1.0f;
            CreateBall(new Vector3(-4.5f + i*0.5f, 6.0f + 2.0f*i, 0.0f), // Position
                       new Vector3(0.0f          , 0.0f         , 0.0f), // Velocity
                       r);                                               // Radius
        }

        var dist = 8.5f;
        for (var i = 0; i < 2; i++) {
            var a = i - 1.0f;
            var b = a + 0.5f;
            CreateBox(5.0f*Vector3.Right + dist*a*Vector3.Up,
                      new Vector3(6.0f, 0.5f, 1.0f),
                      Vector3.Backward, 20.0f);
            CreateBox(5.0f*Vector3.Left + dist*b*Vector3.Up,
                      new Vector3(6.0f, 0.5f, 1.0f),
                      Vector3.Backward, -20.0f);
        }
    }

    /// <summary>Draws the scene by invoking the <see cref="EcsSystem.Draw"/>
    ///          method on all systems in the scene.</summary>
    /// <param name="t">The total game time, in seconds.</param>
    /// <param name="dt">The game time, in seconds, since the last call to this method.</param>
    public override void Draw(float t, float dt)  {
        Game1.Inst.GraphicsDevice.Clear(Color.White);
        base.Draw(t, dt);
    }

    //--------------------------------------
    // NON-PUBLIC METHODS
    //--------------------------------------

    /// <summary>Creates a new ball in the scene with the given position and velocity.</summary>
    /// <param name="p">The ball position, in world-space.</param>
    /// <param name="v">The initial velocity to give to the ball.</param>
    /// <param name="r">The ball radius.</param>
    private int CreateBall(Vector3 p, Vector3 v, float r=1.0f) {
        var ball = AddEntity();

        AddComponent(ball, new CBody { Aabb        = new BoundingBox(-r*Vector3.One, r*Vector3.One),
                                       Radius      = r,
                                       LinDrag     = 0.1f,
                                       Velocity    = v,
                                       Restitution = 0.3f});

        AddComponent(ball, new CTransform { Position = p,
                                            Rotation = Matrix.Identity,
                                            Scale    = r*Vector3.One });

        AddComponent<C3DRenderable>(ball, new CImportedModel {
            model  = Game1.Inst.Content.Load<Model>("Models/DummySphere")
        });

        return ball;
    }

    private void CreateBox(Vector3 pos, Vector3 dim, Vector3 rotAxis, float rotDeg) {
        var rotRad = MathHelper.ToRadians(rotDeg);

        var box1 = AddEntity();

        AddComponent<C3DRenderable>(box1, new CImportedModel {
            model  = Game1.Inst.Content.Load<Model>("Models/DummyBox")
        });

        var rot = Matrix.CreateFromAxisAngle(rotAxis, rotRad);
        var invRot = Matrix.Invert(rot);

        AddComponent(box1, new CTransform { Position = pos,
                                            Rotation = rot,
                                            Scale    = dim });

        AddComponent(box1, new CBox { Box = new BoundingBox(-dim, dim), InvTransf = invRot });
    }

    /// <summary>Sets up the camera.</summary>
    /// <param name="fovDeg">The camera field of view, in degrees.</param>
    /// <param name="zNear">The Z-near clip plane, in meters from the camera.</param>
    /// <param name="zFar">The Z-far clip plane, in meters from the camera..</param>
    private int InitCam(float fovDeg=60.0f, float zNear=0.01f, float zFar=1000.0f) {
        var aspect = Game1.Inst.GraphicsDevice.Viewport.AspectRatio;
        var cam    = AddEntity();
        var fovRad = fovDeg*2.0f*MathHelper.Pi/360.0f;
        var proj   = Matrix.CreatePerspectiveFieldOfView(fovRad, aspect, zNear, zFar);

        AddComponent(cam, new CCamera { ClipProjection = proj,
                                        Projection     = proj });

        AddComponent(cam, new CTransform { Position = new Vector3(0.0f, 0.0f, 18.0f),
                                           Rotation = Matrix.Identity,
                                           Scale    = Vector3.One });

        return cam;
    }
}

}
