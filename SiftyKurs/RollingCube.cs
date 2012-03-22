using System;
using Sifteo;
using SiftyKurs;

namespace SiftyKurs
{
  public class RollingCube: GameCube
  {
    private int _x, _y;
    private bool isRestingOnHorizontalBorder, isRestingOnVerticalBorder;
    private readonly int _height, _width;
    private readonly string _STONE;
    private static readonly int NO_MOTION_THRESHOLD = 1;
    private bool printingHealthPoint;
    private float endPrintingTime;
    private int healthPoints;
    private double _speed_x;
    private double _speed_y;

    private CubeSwitchHandler switcher;

    public double SpeedX{
      get { return _speed_x;}
    }
    public double SpeedY{
      get { return _speed_y;}
    }
    public int X{
      get {return _x;}
    }
    public int Y{
      get {return _y;}
    }

    public RollingCube (Cube c): this(c,MIDDLE, MIDDLE,0,0) {}

    public RollingCube(Cube c, int x, int y): this(c, x, y, 0,0){}

    public RollingCube(Cube c, int x, int y, double initialSpeed_x, double initialSpeed_y) : base(c)
    {
      //this constructor initialize all the variable
      //no need to modify this code!
      _c.userData = this;
      _x = x;
      _y = y;
      isRestingOnVerticalBorder = false; isRestingOnVerticalBorder = false;
      _speed_x = initialSpeed_x;
      _speed_y = initialSpeed_y;
      _STONE = "BallStone";
      //_STONE = "RollingStone";
      _height = Cube.SCREEN_HEIGHT / 4;
      _width = Cube.SCREEN_WIDTH / 4;
    }


    #region Events and Handlers
    //This is the C# way to create events and handlers,
    // you can ask if you want to know more about this mecanism
    internal delegate void BallGoingOutHandler(Cube.Side s);
    internal event BallGoingOutHandler BallGoingOutEvent;

    public delegate void BounceHandler();
    public event BounceHandler BallBouncingEvent;

    public delegate void BallSwitchedCubeHandler();
    public event BallSwitchedCubeHandler BallSwitchedCubeEvent;
    #endregion


    override
    internal void SetupCube ()
    {
      /*
       * First step when setting up the cube:
       *  - make sure that not a single old event is still around and messing up!
       *
       * this is done by the following line
       */
      BallGoingOutEvent = null;
      printingHealthPoint = false;
      //As a RollingCube is nothing else than a GameCube with a rolling stone inside,
      // we need to call the GameCube setup method:
      base.SetupCube();
      //we can now "paint" the cube
      DrawCube ();

      //we also need to setup the event registration :
      InitEventRegistration();

      //checking if any other cube is already our neighbor
      if(!_c.Neighbors.IsEmpty){
        foreach(Cube c in _c.Neighbors){
          if(c != null){
            Log.Debug("this rolling cube is already connected with another one");
            InitCubeSwitchingRegistration(_c, _c.Neighbors.SideOf(c), c, c.Neighbors.SideOf(_c));
          }
        }
      }
      //that's it for now

      Log.Debug("a rolling cube has been set up: speed("+_speed_x+";"+_speed_y+") position("+_x+";"+_y+")");
    }

    private void InitEventRegistration(){
      _c.NeighborAddEvent += PrepareCubeSwitching;
//      _c.TiltEvent += TiltHandler;
    }

    private void PrepareCubeSwitching(Cube c, Cube.Side s, Cube neighbor, Cube.Side neighborSide)
    {
      InitCubeSwitchingRegistration(c, s, neighbor, neighborSide);
    }
//    private void TiltHandler(Cube c, int x, int y, int z){
//      //the tilt state changed : let's restart the acceleration time.
//      _tiltDuration = 0;
//    }

    private void InitCubeSwitchingRegistration (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      Log.Debug("InitCubeSwitchingRegistration! cube side : "+side+" and neighbor side: "+neighborSide);
      //add a special tilt event listener that will swap the ball from this cube to the other
      switcher = new CubeSwitchHandler(this, side, neighbor, neighborSide);
      //TODO switch must listen the tick to be able to switch the ball of cube...
      this.BallGoingOutEvent +=  switcher.HandleGoingOutBallEvent;
      _c.NeighborRemoveEvent += switcher.Remove;
    }


    public void UpdatePosition(float DeltaFrame){
      Tilt t = Helper.NormalizeTilt(_c.Tilt);
//      _tiltDuration = _tiltDuration + DeltaFrame;
      //according to the tilt change the speed:
      double inclinationFactor = (2.0-t.Z)/2.0; //this will yield 1 or 0.5 depending of how much is the cube inclinated
      if(!isRestingOnVerticalBorder){
        _speed_x = _speed_x + speedFactor*t.X*inclinationFactor; //NOTE to have it more realistic we could use a counter to know since when is the cube tilted in this position
      }
      if(!isRestingOnHorizontalBorder){
        _speed_y = _speed_y + speedFactor*t.Y*inclinationFactor;
      }

      //now that we have the speed we just need to move the stone
      MoveTheStone(DeltaFrame);
    }

    public void MoveTheStone(float deltaTime){
      try{
        //speed is given in pixel per frame
        // and delta is given in frame which is perfect:
        int temp_x, temp_y;
        temp_x = _x + (int) (_speed_x*deltaTime);
        temp_y = _y + (int) (_speed_y*deltaTime);
        if(temp_x != _x || temp_y != _y){
          //Log.Debug(deltaTime+" frame(s) spent since last Refresh");
          //Log.Debug("current speed is: ("+_speed_x+","+_speed_y+")");
          _x = temp_x;
          _y = temp_y;

          //we just need to make sure that now _x and _y are still inside the limits...
          //Log.Debug("new ball position before calling checkBall -> "+_x+"x"+_y);
          CheckBallPosition();
          //Log.Debug("new ball position after calling checkBall -> "+_x+"x"+_y);
        }
      }catch (Exception e){
        Log.Debug("if the following message says that the ball switched cubes then it's perfect... \n\t-->"+e.Message);
      }
    }
    public void RefreshDrawing(float playedTime){
      if(printingHealthPoint){
        DrawCube(false);
        Helper.PrintNumberOnACube(_c, healthPoints, false);
        _c.Paint();
        if(playedTime > endPrintingTime){
          printingHealthPoint = false;
        }
      }else{
        DrawCube();
      }
    }
    public void PrintHealthPoint(int healthPoints, float endTime){
      printingHealthPoint = true;
      endPrintingTime = endTime;
      this.healthPoints = healthPoints;
    }
    public void ResetCube(){
      _speed_x = 0;
      _speed_y = 0;
      _x = MIDDLE;
      _y = MIDDLE;
      this.SetupCube();
    }

    override
    protected void DrawCube (bool repaint)
    {
      base.DrawCube(false);
      int pic_x = _x - _width / 2;
      int pic_y = _y - _height / 2;
      //Log.Debug ("placement values: x->" + _x + ", y->" + _y + ", pic_x->" + pic_x + ", pic_y->" + pic_y);
      _c.Image (_STONE, pic_x, pic_y, 0, 0, _width, _height, 1, 0);
      if (repaint) {
        _c.Paint ();
      }
    }

    private void CheckBallPosition(){
      isRestingOnVerticalBorder = false;
      isRestingOnHorizontalBorder = false;
      //let's check X first:
      if(_x < MIDDLE){
        int min_x = base.MinX + _width/2;//the ball need to be inside the cube hence the _widht/2
        if( _x < min_x){
          if(GoingTowardTheSide(Cube.Side.LEFT)){
            if(BallGoingOutEvent != null){
              BallGoingOutEvent(Cube.Side.LEFT);
            }
            //_speed_x = 0;//TODO find a formula to make the ball "bounce"
            //The BallOnBorder event MUST be send BEFORE changing the speed
            //... otherwise the ball will move in the wrong direction on the other cube in case of switch
            //... and it will loop like crazy...
            //something like speedFactor*0.5*_speed_x*-1 should do it;
            _speed_x = - 0.5*_speed_x;
            //if we reach this part it means that no switching has been done
            //so we are sure that the stone bounced:
            if(Math.Abs(SpeedX) < NO_MOTION_THRESHOLD){
              isRestingOnVerticalBorder = true;
            }else{
              isRestingOnVerticalBorder = false;
              if(BallBouncingEvent != null){
                BallBouncingEvent();
              }
            }
          }
          _x = min_x;
        }//else it's fine!
      }else{ //i.e. _x >= 0
        int max_x = base.MaxX - _width/2;
        if(_x > max_x){
          if(GoingTowardTheSide(Cube.Side.RIGHT)){
            if(BallGoingOutEvent != null){
              BallGoingOutEvent(Cube.Side.RIGHT);
              Log.Debug("after a ball on the right border ");
            }
            _speed_x = - 0.5*_speed_x;
            //if we reach this part it means that no switching has been done
            //so we are sure that the stone bounced:
            if(Math.Abs(SpeedX) < NO_MOTION_THRESHOLD){
              isRestingOnVerticalBorder = true;
            }else{
              isRestingOnVerticalBorder = false;
              if(BallBouncingEvent != null){
                BallBouncingEvent();
              }
            }
          }
          _x = max_x;
        }//else it's fine!
      }

      //let's check Y now:
      if(_y < MIDDLE){
        int min_y = base.MinY + _height/2;
        if( _y < min_y){
          if(GoingTowardTheSide(Cube.Side.TOP)){
            if(BallGoingOutEvent != null){
              BallGoingOutEvent(Cube.Side.TOP);
            }
            _speed_y = - 0.75*_speed_y;
            //if we reach this part it means that no switching has been done
            //so we are sure that the stone bounced:
            if(Math.Abs(SpeedY) < NO_MOTION_THRESHOLD){
              isRestingOnHorizontalBorder = true;
            }else{
              isRestingOnHorizontalBorder = false;
              if(BallBouncingEvent != null){
                BallBouncingEvent();
              }
            }
          }
          _y = min_y;
        }//else it's fine
      }else{
        int max_y = base.MaxY - _height/2;
        if( _y > max_y){
          if(GoingTowardTheSide(Cube.Side.BOTTOM)){
            if(BallGoingOutEvent != null){
              BallGoingOutEvent(Cube.Side.BOTTOM);
            }
            _speed_y = - 0.75*_speed_y;
            //if we reach this part it means that no switching has been done
            //so we are sure that the stone bounced:
            if(Math.Abs(SpeedY) < NO_MOTION_THRESHOLD){
              isRestingOnHorizontalBorder = true;
            }else{
              isRestingOnHorizontalBorder = false;
              if(BallBouncingEvent != null){
                BallBouncingEvent();
              }
            }
          }
          _y = max_y;
        }
      }
    }

    internal void MoveToNewCube(Cube futurRollingCube, int futur_x, int futur_y, double futur_speed_x, double futur_speed_y, double futur_speedFactor){
      _c.NeighborAddEvent -= InitCubeSwitchingRegistration;
      //switcher.Remove();
      Log.Debug("Before swithing cube the old values are: speed("+_speed_x+";"+_speed_y+") position("+_x+";"+_y+")");

      this._c = futurRollingCube;
      _x = futur_x;
      _y = futur_y;
      _speed_x = futur_speed_x;
      _speed_y = futur_speed_y;
      speedFactor = futur_speedFactor;
      this.SetupCube();
      BallSwitchedCubeEvent();
    }

    internal bool GoingTowardTheSide(Cube.Side s){
      bool goingTowardTheBorder = false;
      switch(s){
      case Cube.Side.BOTTOM:
        goingTowardTheBorder = (SpeedY > 0);
        break;
      case Cube.Side.TOP:
        goingTowardTheBorder = (SpeedY < 0);
        break;
      case Cube.Side.LEFT:
        goingTowardTheBorder = (SpeedX < 0);
        break;
      case Cube.Side.RIGHT:
        goingTowardTheBorder = (SpeedX > 0);
        break;
      default:
        break;
      }
      return goingTowardTheBorder;
    }
  }


}

