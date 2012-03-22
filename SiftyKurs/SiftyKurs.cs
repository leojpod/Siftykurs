using Sifteo;
using System;

namespace SiftyKurs
{
  /**
   * Welcome in this Mini course!
   *
   * The main job will be to code some part of this application in this file.
   * As a start you should quickly go through the instructions
   */

  public class SiftyKurs : BaseApp
  {
    //we need to keep track of the cube holding the rolling stone:
    private RollingCube rc;

    //this is our healthPoint counter
    private int stoneHealthPoints;

    //this is our score counter
    private int score;

    //this is just to keep track of the time spent
    private float playedTime;

    //this is a bit of C# event magic...
    //basically we need to create a type of handler for our new event.
    //this type will be used by event handlers as we will see later.

    //this line below describes a new type of handler that we received a float value from time to time
    private delegate void NewTimeHandler(float newTime);
    //this line below declares a new event that needs handler of the previously declared type
    private event NewTimeHandler NewTimeEvent;
    //this mecanism is not so clear but it's easier to explain it by oral than by writing lines of comments

    //this describes another type of event
    private delegate void GameOverHandler();
    private event GameOverHandler GameOverEvent;

    //this is a sifteo thing we don't care about that
    override public int FrameRate
    {
      get { return 20; }
    }

    // called during intitialization, before the game has started to run
    override public void Setup()
    {
      Log.Debug("Setup()");
      /*
       * We want to initialize all the cube with a GameCube object and then select one cube to put
       * the stone on it (i.e. creating a RollingCube on one cube)
       *
       */
      foreach(Cube c in this.CubeSet){
        GameCube gc = new GameCube(c);
        gc.SetupCube();
        Log.Debug("one more GameCube ready!");
      }

      Cube aCube = this.CubeSet[0];
      rc = new RollingCube(aCube);
      rc.SetupCube();
      Log.Debug("the rolling cube has the id: "+rc.C.UniqueId);

      //Setting up the game here:
      /*
       * The aim of the game is to carry the stone from one cube to the other as much as possible
       * We need to avoid bounces because they remove health point from our stone
       * Try to score as much point as possible! The score will be printed on the cubes for 1 second each time
       * the bounce counter reach 0
       */
      //first let's initialize our counters:
      stoneHealthPoints = 10;
      score = 0;
      playedTime = 0;

      //we want the method SimpleBounce to be called each time a BallBouncingEvent is fired
      //this is done by using the following operator +=
      //to prevent SimpleBounce to be called anyfurther by BallBouncingEvent one can use the -= operator
      rc.BallBouncingEvent += SimpleBounce;

      //this will be removed for the course version
      rc.BallBouncingEvent += DecreaseBallHP;

      //We want to call the method SimpleSwitch each time an event BallSwitchedCubeEvent is fired
      rc.BallSwitchedCubeEvent += SimpleSwitch;

      //this will be removed too
      rc.BallSwitchedCubeEvent += AddPoint;

      //We want to reset the game each time the game is over
      //so we can play all day long and have fun

      this.GameOverEvent += SimpleGameOver;
      //this will be removed too
      this.GameOverEvent += ResetGame;
    }

    public void SimpleBounce(){
      Log.Info("bounce");
    }
    public void SimpleSwitch(){
      Log.Info("switch");
    }
    public void SimpleGameOver(){
      Log.Info("game over");
    }

    /*
     * This method removes one HP from the ball and fires a GameOverEvent
     * if the HP are below or equal to zero
     */
    public void DecreaseBallHP(){
      //we want to decrease the stone health point by one
      //to remove
      stoneHealthPoints = stoneHealthPoints - 1;
      //and we want to print the healthPoint on the stone

      //to remove
      PrintHealthPoint();

      if(stoneHealthPoints <= 0){
        //to remove
        if(GameOverEvent != null){
          GameOverEvent();
        }
      }
    }

    /*
     * This method prints the current value of stoneHealthPoints on a cube for one second
     */
    public void PrintHealthPoint(){
      //we use the rollingCube object to do that
      rc.PrintHealthPoint(stoneHealthPoints, playedTime + 1);
    }

    /*
     * This method prints the score on all the cubes for a second
     */
    public void PrintScore(){
      CubeSet setOfCubes = this.CubeSet;
      //we want to go through all the cubes in our setOfCubes and to print the score on each of them

      //to remove
      foreach(Cube c in setOfCubes){
        PrintNumberOnACube(c, score);
      }
    }

    /*
     * This method prints a number on a cube for one second
     */
    public void PrintNumberOnACube(Cube c, int number){
      //this method use a more advance C# synthax
      //but we don't care as long as it does the job
      float endTime = playedTime + 1;
      NewTimeHandler printMagic = delegate(float newTime) {
        if(newTime < endTime){
          Helper.PrintNumberOnACube(c, number, true);
          c.Paint();
        }else{
          NewTimeEvent -= printMagic;
        }
      };
      NewTimeEvent += printMagic;
    }

    /*
     * This method prepares the game for a new round:
     *  - it prints the score on the cubes
     *  - it sets back the score to 0
     *  - it sets back the HP to 10
     *  - and it call the RollingCube to reset it. (RollingCube has a method called ResetCube for that)
     */
    public void ResetGame(){
      //to remove
      PrintScore();
      score = 0;
      stoneHealthPoints = 10;
      rc.ResetCube();
    }

    /*
     * This method adds one point to the current score
     */
    public void AddPoint(){
      //to remove
      score = score + 1;
    }

    //This is a sifteo method: it is called each time the cubes finished to compute the previous tick
    //basically, as soon as this method is over, it is called again by the sifteo enviromnent
    override public void Tick()
    {
      //we need to update the total time played
      playedTime = playedTime + this.DeltaTime;
      //Log.Debug("Tick()");
      //we want the rolling cube to move the ball according to the cube state (tilt status)
      rc.UpdatePosition(this.DeltaTime * this.FrameRate);
      //this check that the cubes are ready to receive information through the radio transmetter.
      //if not then we should not refresh the cubes to avoid "stacking" drawing commands and making the game lag
      if(this.IsIdle){
        //so if we can redraw the cube let's do it!
        rc.RefreshDrawing(playedTime);
        //the following lines check is there is any handler for the NewTimeEvent and fire it if there is at least one
        //this event is used by the printNumberMethod
        if(NewTimeEvent != null){
          NewTimeEvent(playedTime); //let's fire a NewTimeEvent
        }
      }else{
        //we wait to avoid killing the cubes
        //Log.Debug("connection busy");
      }

    }

    // development mode only
    // start SiftyKurss as an executable and run it, waiting for Siftrunner to connect
    static void Main(string[] args) { new SiftyKurs().Run(); }
  }
}

