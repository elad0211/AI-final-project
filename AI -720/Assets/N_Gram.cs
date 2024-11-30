
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Represents the frequency data of moves for a specific n-gram sequence
public class KeyDataRecord
{
    public Dictionary<string, int> counts = new Dictionary<string, int>(); // Tracks the frequency of each move (rock, paper, scissors)
    public int total = 0; // Total occurrences for this sequence

    // Constructor initializes counts dictionary with default values for each move type
    public KeyDataRecord()
    {
        counts.Add("s", 0); // Frequency for "shoot"
        counts.Add("r", 0); // Frequency for "reload"
        counts.Add("b", 0); // Frequency for "block"
        counts.Add("z", 0); // Frequency for "bazuka"
        total = 0;
    }
}

// Implements the n-gram model to predict the player's next move based on past sequences
public class N_Gram : AI_BehaviorTree
{
    public Dictionary<string, KeyDataRecord> data = new Dictionary<string, KeyDataRecord>(); // Holds frequency data for each n-gram sequence
    public string currentRecord = ""; // Tracks the most recent sequence of moves
    int nValue = 2; // The length of sequences to track (n-gram size)

    // Registers a new sequence in the n-gram data
    public void register(string sequence)
    {
        char playerAction = sequence[sequence.Length - 1]; // The last move in the sequence
        string key = sequence.Remove(sequence.Length - 1, 1); // The sequence key, excluding the latest move

        // Ensure the key exists in data, and add it if not
        if (!data.ContainsKey(key))
        {
            data[key] = new KeyDataRecord();
        }

        // Increment the count for the specific move (action) in this sequence
        data[key].counts[playerAction.ToString()] += 1;
        data[key].total += 1;
    }

    // Logs the player's input move and updates the current sequence
    public void LogInput(char s)
    {
        // Append the new move to the current sequence, removing the oldest move if necessary
        if (currentRecord.Length < nValue)
        {
            currentRecord += s;
        }
        else
        {
            currentRecord = currentRecord.Remove(0, 1) + s;
        }

        // Only register the sequence when it has reached the required nValue length
        if (currentRecord.Length == nValue)
        {
            register(currentRecord);
        }
    }

    // Predicts the player's most likely next move based on past patterns
    public GameNamespace.Action GetMostLikly()
    {
        // Return a random move if the current record is empty (i.e., no data to predict)
        if (currentRecord.Length == 0)
             return GuessActionAINoData();

        // Use the current sequence excluding the latest move as the key
        string key = currentRecord.Remove(0, 1);
        if (!data.ContainsKey(key))
            return GuessActionAINoData();

        // Retrieve the recorded data for this sequence
        KeyDataRecord keyData = data[key];

        GameNamespace.Action expectedMove = GameNamespace.Action.b; // Default to a random move
        int highestValue = 0;

        // Determine which move has the highest frequency in this sequence
        if (keyData.counts["s"] > highestValue)
        {
            expectedMove = GameNamespace.Action.s;
            highestValue = keyData.counts["s"];
        }
        if (keyData.counts["r"] > highestValue)
        {
            expectedMove = GameNamespace.Action.r;
            highestValue = keyData.counts["r"];
        }
        if (keyData.counts["b"] > highestValue)
        {
            expectedMove = GameNamespace.Action.b;
            highestValue = keyData.counts["b"];
        }

        return expectedMove; // Return the move with the highest probability
    }
    public override GameNamespace.Action ChooseActionAI()
    {
        int playerBullets = manager.player1.bullets;

        //Check if Bazooka action is available for AI
        if (bullets == 6)
        {
            Debug.Log("Ngram reached 5 ammo and decided to use Bazooka");
            action = GameNamespace.Action.z;
            return action;
        }

        //If neither player nor AI have bullets, reload.
        if (playerBullets == 0 && bullets == 0)
        {
            Debug.Log("Both player have no ammo, Ngram chose to reload");

            action = GameNamespace.Action.r;
            return action;
        }

       
            //If AI has bullets but player does not
            // OR player is about to get bazooka, shoot
            if ( (playerBullets == 5 || playerBullets == 0) && bullets >= 2)
            {
                Debug.Log("Enemy player almost reached Bazuka, Ngram chose to shot");
                 action = GameNamespace.Action.s;
                return action;

            }

            //else random chance action
            else
            {
                GameNamespace.Action expectedAction = GetMostLikly();
                if (expectedAction == GameNamespace.Action.r && bullets > 0)
                {
                    Debug.Log("Ngram is expecting a reload and ready to fire");

                action = GameNamespace.Action.s;
                return action;
            }
                else if (expectedAction == GameNamespace.Action.r || expectedAction == GameNamespace.Action.b)
                {
                  if(  expectedAction == GameNamespace.Action.r)
                       Debug.Log ( " Ngram is expecting a reload but has no bullets so chose to reload");
                  else
                        Debug.Log("Ngram is expecting a block so chose to reload");

                action = GameNamespace.Action.r;
                return action;
            }
                
            }
        
        Debug.Log("Ngram is expecting a shot and chose to block");
        action = GameNamespace.Action.b;
        return GameNamespace.Action.b;



    }

    public  GameNamespace.Action GuessActionAINoData()
    {
        int playerBullets = manager.player1.bullets;

       

        // If AI has ammo
        if (bullets >= 1)
        {

            
                int actionChance = random.Next(1, 101);
                if (actionChance <= 50) //40% chance block
                {
                Debug.Log("Not enough data (guess block)");

                return GameNamespace.Action.b;

            }
            else if (actionChance <= 80) //30% chance reload
                {
                Debug.Log("Not enough data (guess reload)");

                return GameNamespace.Action.r;
                }
                else //30% chance shoot
                {
                Debug.Log("Not enough data (guess shoot)");

                return GameNamespace.Action.s;
                }
            
        }
        //if AI has no ammo, random chance between block & reload
        else
        {
            int actionChance = random.Next(1, 101);

            if (actionChance <= 50) //50% chance reload
            {
                Debug.Log("Not enough data (guess reload)");
                return GameNamespace.Action.r;
            }
            else //50% chance block
            {
                Debug.Log("Not enough data (guess block)");
                return GameNamespace.Action.b;
                ;
            }

        }


    }


}
