interface IScorer
{
    /// <summary>
    ///  Returns a value between 0 and 1. This should be used for all scores to ensure standardisation
    /// </summary>
    protected static double normaliseScore(double value, double min, double max)
    {
        return Math.Clamp((value - min) / (max - min), 0, 1);
    }

    protected static double combineScores(List<(double, double)> scoreWeightingList)
    {
        double totalWeightedScore = 0;

        // Iterate through each score
        foreach ((double, double) scoreWeighting in scoreWeightingList)
        {
            // Expand the pair
            double score = scoreWeighting.Item1;
            double weighting = scoreWeighting.Item2;

            // keep calculate the weighted total
            totalWeightedScore = totalWeightedScore + (score*weighting);
        }

        return totalWeightedScore;
    }
}