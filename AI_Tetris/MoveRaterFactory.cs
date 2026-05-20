using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

class MoveRaterFactory
{

    private MoveRater? lastMoveRater = null;
    private readonly string resultsFile = "results.csv";
    private readonly List<MoveRater> population = new List<MoveRater>();
    

    public MoveRaterFactory()
    {
        loadPrevResults();
    }

    public MoveRater createRandomMoveRater()
    {
        // Generate random weights
        double nbRowsClearedScoreWeight = Random.Shared.NextDouble();
        double avgHeightScoreWeight = Random.Shared.NextDouble();
        double nbGapsScoreWeight = Random.Shared.NextDouble();
        double elevationChangeScoreWeight = Random.Shared.NextDouble();

        // Normalise the weights to sum to 1
        int targetSum = 1;
        double initialSum = nbRowsClearedScoreWeight + avgHeightScoreWeight + nbGapsScoreWeight + elevationChangeScoreWeight;
        double factor = targetSum/initialSum;

        nbRowsClearedScoreWeight = nbRowsClearedScoreWeight*factor;
        avgHeightScoreWeight = avgHeightScoreWeight*factor;
        nbGapsScoreWeight = nbGapsScoreWeight*factor;
        elevationChangeScoreWeight = elevationChangeScoreWeight*factor;

        // Check the sum
        double finalSum = nbRowsClearedScoreWeight + avgHeightScoreWeight + nbGapsScoreWeight + elevationChangeScoreWeight;
        double tolerance = 0.00005;
        if (Math.Abs(finalSum - 1) > tolerance)
        {
            throw new Exception("Weights normalisation failed, they should sum to 1, got: " + finalSum.ToString());
        }

        // Create the MoveRater
        MoveRater moveRater = new MoveRater(nbRowsClearedScoreWeight, avgHeightScoreWeight, nbGapsScoreWeight, elevationChangeScoreWeight);
        lastMoveRater = moveRater;
        return moveRater;
    }

    public MoveRater getBestMoveRater()
    {
        return population[0];
    }

    public MoveRater createCandidateMoveRater()
    {
        // Select the parents by tournament selection
        int nbCompetitors = 3;
        int nbTournamentParents = 3;
        double selectionThreshold = 0.25; 
        List<MoveRater> parents = selectParents(nbCompetitors, nbTournamentParents, selectionThreshold);

        // Add a random parent in 20% of the time
        double randomParentChance = 1;
        if (Random.Shared.NextDouble() < randomParentChance)
        {
            parents.Add(createRandomMoveRater());
        }

        // Create the child
        MoveRater child = createChild(parents);

        // Return the child
        lastMoveRater = child;
        return child;
    }

    private MoveRater createChild(List<MoveRater> parents)
    {
        // Define the array of genes for the child
        int nbGenes = parents[0].getGenes().Length;
        double[] childGenes = new double[nbGenes];

        // Calculate the average of each gene from all parents
        for (int geneIndex = 0; geneIndex < nbGenes; geneIndex++)
        {
            double parentsGenesTotal = 0;
            foreach (MoveRater parent in parents)
            {
                parentsGenesTotal += parent.getGenes()[geneIndex];
            }
            double avgGene = parentsGenesTotal / parents.Count;
            childGenes[geneIndex] = avgGene;
        }

        // Create and return a child with the calculated genes
        MoveRater child = new MoveRater(childGenes);
        return child;
    }

    private List<MoveRater> selectParents(int nbCompetitors, int nbParents, double selectionThreshold)
    {
        var rnd = new Random();
        List<MoveRater> parents = new List<MoveRater>();
        int maxIndex = (int)Math.Round(selectionThreshold*this.population.Count);

        // Iterate for the number of parents you'd like
        for(int parIndex = 0; parIndex < nbParents; parIndex++)
        {
            // Compare n competitors and choose the fittest to be the parent
            MoveRater? winningCompetitor = null;
            for(int cmpIndex = 0; cmpIndex < nbCompetitors; cmpIndex++) 
            {
                int popIndex = rnd.Next(0, maxIndex);
                MoveRater challenger = this.population[popIndex];
                if (winningCompetitor == null || challenger.fitness > winningCompetitor.fitness)
                {
                    winningCompetitor = challenger;
                }
            }
            parents.Add(winningCompetitor!);
        }
        return parents;
    }

    private void loadPrevResults()
    {
        // Read the entire file except the header row
        List<double[]> rows = new List<double[]>();
        if (File.Exists(resultsFile))
        {
        rows = File.ReadAllLines(resultsFile).Skip(1)
            .Select(line => line.Split(',')
                .Select(v => double.Parse(v, CultureInfo.InvariantCulture))
                .ToArray())
            .ToList();                
        }

        foreach (double[] row in rows)
        {
           MoveRater moveRater = new MoveRater(row[0], row[1], row[2], row[3], row[4]); 
           this.population.Add(moveRater);
        }
    }

    public void saveResults(double timeSurvived)
    {
        if (lastMoveRater == null)
        {
            throw new Exception("No move rater created");
        }
        else
        {
            // Define the new row to be added to the file
            double[] moveRaterWeights =lastMoveRater.getGenes();
            double[] newEntry = moveRaterWeights.Append(timeSurvived).ToArray();
            MoveRater moveRater = new MoveRater(newEntry[0], newEntry[1], newEntry[2], newEntry[3], newEntry[4]); 
            this.population.Add(moveRater);
            
            // Read the entire file except the header row
            List<double[]> entries = new List<double[]>();
            if (File.Exists(resultsFile))
            {
            entries = File.ReadAllLines(resultsFile).Skip(1)
                .Select(line => line.Split(',')
                    .Select(v => double.Parse(v, CultureInfo.InvariantCulture))
                    .ToArray())
                .ToList();                
            }

            // Insert the row
            int insertionIndex = entries.FindIndex(row => row[row.Length-1] < timeSurvived);
            if (insertionIndex == -1)
            {
                entries.Add(newEntry);
            }
            else
            {
                entries.Insert(insertionIndex, newEntry);
            }

            // Overwrite the whole file
            string headerRow = lastMoveRater.getWeightsTitles()+",timeSurvived\n";
            File.WriteAllText(resultsFile, headerRow);

            File.AppendAllLines(resultsFile,
                entries.Select(r =>
                    string.Join(",",
                        r.Select(v => v.ToString(CultureInfo.InvariantCulture)))));
        }
    }

}