using Sifteo;
using System;

namespace SiftyKurss
{
  public class SiftyKurss : BaseApp
  {

    override public int FrameRate
    {
      get { return 20; }
    }

    // called during intitialization, before the game has started to run
    override public void Setup()
    {
      Log.Debug("Setup()");
      //init 3 cubes with basic actions
      //put the stone randomly on one of them
      //wait until the cube with the stone is clicked
      
      //but for now let´s just create one cube and makes a stone roll on it...
      Cube c = this.CubeSet[0];
      RollingCube rc = new RollingCube(c);
    }

    override public void Tick()
    {
      Log.Debug("Tick()");
    }

    // development mode only
    // start SiftyKurss as an executable and run it, waiting for Siftrunner to connect
    static void Main(string[] args) { new SiftyKurss().Run(); }
  }
}

