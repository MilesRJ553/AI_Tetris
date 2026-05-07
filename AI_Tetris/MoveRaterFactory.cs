class MoveRaterFactory()
{
    
    public MoveRater createRandomMoveRater()
    {
        double nbRowsClearedScoreWeight = Random.Shared.NextDouble();
        double avgHeightScoreWeight = Random.Shared.NextDouble();
        double nbGapsScoreWeight = Random.Shared.NextDouble();
        double elevationChangeScoreWeight = Random.Shared.NextDouble();

        MoveRater moveRater = new MoveRater(nbRowsClearedScoreWeight, avgHeightScoreWeight, nbGapsScoreWeight, elevationChangeScoreWeight);
        return moveRater;
    }

}