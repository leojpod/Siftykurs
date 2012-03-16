using System;
using Sifteo;

namespace SiftyKurs
{
  public class GameCube
  {
    protected Cube _c;
    private string _background;
    private string _border;
    protected readonly int _borderSize;
    protected int _borderLength{
      get; private set;
    }
    protected bool[] _visibleBorders{
      get; private set;
    }
    internal int MinX {
      get; private set;
    }
    internal int MaxX {
      get; private set;
    }
    internal int MinY {
      get; private set;
    }
    internal int MaxY {
      get; private set;
    }

    public static readonly int MIDDLE = Cube.SCREEN_WIDTH / 2;

    public enum Side{TOP,LEFT,BOTTOM,RIGHT};

    public Cube C {
      get { return _c;}
    }
    public readonly double speedFactor;


    public GameCube (Cube c)
    {
      _c = c;
      _c.userData = this;
      _border = "borders";
      _background = "pavement";
      _borderSize = 8;
      _borderLength = Cube.SCREEN_WIDTH;
      _visibleBorders = new bool[]{true,true,true,true};
      MinX = _borderSize; MinY = _borderSize;
      MaxX = Cube.SCREEN_MAX_X - _borderSize;
      MaxY = Cube.SCREEN_MAX_Y - _borderSize;
      speedFactor = 10;
    }

    protected void SetBorderDisplay(Cube.Side s, bool visible){
      switch(s){
      case Cube.Side.TOP:
        _visibleBorders[(int)GameCube.Side.TOP] = visible;
        MinY = visible? _borderSize :  0;
        break;
      case Cube.Side.LEFT:
        _visibleBorders[(int)GameCube.Side.LEFT] = visible;
        MinX = visible? _borderSize :  0;
        break;
      case Cube.Side.BOTTOM:
        _visibleBorders[(int)GameCube.Side.BOTTOM] = visible;
        MaxY = visible? Cube.SCREEN_MAX_Y-_borderSize :  Cube.SCREEN_MAX_Y;
        break;
      case Cube.Side.RIGHT:
        _visibleBorders[(int)GameCube.Side.RIGHT] = visible;
        MaxX = visible? Cube.SCREEN_MAX_X-_borderSize :  Cube.SCREEN_MAX_X;
        break;
      default:
        break;
      }
    }

    private void initBorder(){
    //let's look for some neighbors...
      if(_c.Neighbors.Bottom != null){
        SetBorderDisplay(Cube.Side.BOTTOM, false);
      }else{
        SetBorderDisplay(Cube.Side.BOTTOM, true);
      }
      if(_c.Neighbors.Left != null){
        SetBorderDisplay(Cube.Side.LEFT, false);
      }else{
        SetBorderDisplay(Cube.Side.LEFT, true);
      }
      if(_c.Neighbors.Right != null){
        SetBorderDisplay(Cube.Side.RIGHT, false);
      }else{
        SetBorderDisplay(Cube.Side.RIGHT, true);
      }
      if(_c.Neighbors.Top != null){
        SetBorderDisplay(Cube.Side.TOP, false);
      }else{
        SetBorderDisplay(Cube.Side.TOP, true);
      }
    }

    internal virtual void SetupCube(){
      _c.ClearEvents ();
      initBorder();
      DrawCube();

      _c.NeighborAddEvent += RemoveBorder;
      _c.NeighborRemoveEvent += PutBackBorder;
    }

    void PutBackBorder (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      //Log.Debug("adding back the border");
      SetBorderDisplay(side,true);
      DrawCube();
    }

    void RemoveBorder (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      //Log.Debug("removing the border");
      SetBorderDisplay(side, false);
      DrawCube();
    }

    protected void DrawCube(){
      DrawCube(true);
    }

    protected virtual void DrawCube(bool repaint){
      //adding the background
      _c.Image (_background, 0, 0, 0, 0, Cube.SCREEN_WIDTH, Cube.SCREEN_HEIGHT, 1, 0);
      //adding the borders:
      if(_visibleBorders[(int)GameCube.Side.TOP]){
        _c.Image(_border, 0, 0, 0, 0, _borderLength, _borderSize, 1, 0 );
      }
      if(_visibleBorders[(int)GameCube.Side.LEFT]){
        _c.Image(_border, 0, 0, 0, 0, _borderSize, _borderLength, 1, 0 );
      }
      if(_visibleBorders[(int)GameCube.Side.BOTTOM]){
        _c.Image(_border, 0, Cube.SCREEN_MAX_Y-_borderSize,
                 0, Cube.SCREEN_MAX_Y-_borderSize, _borderLength, _borderSize, 1, 0 );
      }
      if(_visibleBorders[(int)GameCube.Side.RIGHT]){
        _c.Image(_border, Cube.SCREEN_MAX_X-_borderSize, 0,
                 Cube.SCREEN_MAX_X-_borderSize, 0, _borderSize, _borderLength, 1, 0 );
      }
      if(repaint){
        _c.Paint();
      }
    }
  }
}

