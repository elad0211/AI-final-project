using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameNamespace;
using System.Linq;

public class GameState
{
    // Represents the current state of the game for decision-making
    public int PlayerAmmo { get; set; }        // Amount of ammo the player has
    public int OpponentAmmo { get; set; }      // Amount of ammo the opponent has
    public Action PlayerAction { get; set; }   // The action the player takes in the current turn
    public bool PlayerHasCannon { get; set; }  // Whether the player has a cannon (Bazuka)
    public bool OpponentHasCannon { get; set; } // Whether the opponent has a cannon (Bazuka)
}

public class DecisionTreeNode
{
    public string Feature { get; set; }                         // Feature used to split this node (e.g., "PlayerAmmo")
    public Dictionary<string, DecisionTreeNode> Children { get; set; } = new Dictionary<string, DecisionTreeNode>(); // Child nodes for each feature value
    public GameNamespace.Action? LeafAction { get; set; }        // The action at the leaf node, if this is a leaf node

    public bool IsLeaf => LeafAction.HasValue; // Check if the current node is a leaf node (no further splitting)
}

public class ID3_Base : MonoBehaviour
{
    public enum Action { Reload, Shoot, Block, Bazuka, OutOfAmmo } // Possible actions for a player
    public GameNamespace.Action action;  // Action chosen by the AI
    public int ammoCount;                // Current ammo count for the player
    public bool hasCannon;               // Flag to track if the player has a cannon (Bazuka)

    // Update the player's ammo count and check if they have earned a cannon (Bazuka)
    public void UpdatePlayerAmmo()
    {
        // If the player has accumulated 6 or more ammo, they gain a cannon (Bazuka)
        if (ammoCount >= 6)
        {
            hasCannon = true;  // Player gains cannon (Bazuka) after 6 ammo
        }
        else
        {
            hasCannon = false; // Otherwise, the player doesn't have a cannon
        }
    }

    // Calculate the entropy of a set of game states
    public float CalculateEntropy(List<GameState> states)
    {
        // Group the states by their PlayerAction and count the occurrences of each action
        var actionCounts = states.GroupBy(s => s.PlayerAction)
            .ToDictionary(g => g.Key, g => g.Count());

        float entropy = 0f; // Initialize entropy to zero
        int total = states.Count; // Total number of states

        // Calculate the entropy using the formula: - p(x) * log2(p(x)) for each action probability
        foreach (var count in actionCounts.Values)
        {
            float probability = (float)count / total;  // Probability of the action
            entropy -= probability * Mathf.Log(probability, 2); // Add to entropy
        }

        return entropy; // Return the calculated entropy
    }

    // Calculate the information gain of a feature
    public float CalculateInformationGain(List<GameState> states, string feature)
    {
        // Get all possible distinct values of the selected feature in the current states
        var featureValues = states.Select(s => GetFeatureValue(s, feature)).Distinct().ToList();

        float initialEntropy = CalculateEntropy(states); // Calculate the initial entropy for the full dataset

        float weightedEntropySum = 0f; // Initialize the weighted entropy sum

        // For each feature value, calculate the entropy of the subset of states that have this feature value
        foreach (var value in featureValues)
        {
            // Get the subset of states where the feature value matches
            var subset = states.Where(s => GetFeatureValue(s, feature) == value).ToList();
            float subsetProbability = (float)subset.Count / states.Count;  // Probability of this feature value

            // Add the weighted entropy for this subset
            weightedEntropySum += subsetProbability * CalculateEntropy(subset);
        }

        // Information Gain = Entropy before split - Weighted Entropy after split
        return initialEntropy - weightedEntropySum;
    }

    // Get the value of a feature from a GameState object
    private string GetFeatureValue(GameState state, string feature)
    {
        return feature switch
        {
            "PlayerAmmo" => state.PlayerAmmo.ToString(),  // Return player ammo as a string
            "OpponentAmmo" => state.OpponentAmmo.ToString(), // Return opponent ammo as a string
            "PlayerHasCannon" => state.PlayerHasCannon.ToString(), // Return if player has cannon as string
            "OpponentHasCannon" => state.OpponentHasCannon.ToString(), // Return if opponent has cannon as string
            _ => throw new System.Exception("Unknown feature")  // Throw error for unrecognized feature
        };
    }

    // Build the decision tree based on the entropy and information gain of each feature
    public DecisionTreeNode BuildTree(List<GameState> states, List<string> features)
    {
        // If all examples have the same action, create a leaf node with that action
        if (states.All(s => s.PlayerAction == states[0].PlayerAction))
        {
            return new DecisionTreeNode { LeafAction = states[0].PlayerAction };
        }

        // If there are no more features to split on, return a leaf node with the most common action
        if (features.Count == 0)
        {
            return new DecisionTreeNode { LeafAction = GetMostCommonAction(states) };
        }

        // Select the feature that gives the highest information gain (i.e., reduces uncertainty the most)
        string bestFeature = features.OrderByDescending(f => CalculateInformationGain(states, f)).First();

        var root = new DecisionTreeNode { Feature = bestFeature }; // Create the root node with the best feature

        // Get all distinct values of the selected feature
        var featureValues = states.Select(s => GetFeatureValue(s, bestFeature)).Distinct().ToList();

        // Recursively build the tree for each value of the selected feature
        foreach (var value in featureValues)
        {
            // Get the subset of states that match this feature value
            var subset = states.Where(s => GetFeatureValue(s, bestFeature) == value).ToList();
            var remainingFeatures = features.Where(f => f != bestFeature).ToList();  // Remove the current feature from the list

            // Recursively build the child node for this subset
            root.Children[value] = BuildTree(subset, remainingFeatures);
        }

        return root;  // Return the root of the tree (or subtree)
    }

    // Get the most common action from the states (for leaf nodes)
    private GameNamespace.Action GetMostCommonAction(List<GameState> states)
    {
        // Group the states by PlayerAction and get the action with the highest count
        return states.GroupBy(s => s.PlayerAction)
                     .OrderByDescending(g => g.Count()) // Sort by frequency
                     .First() // Get the most common action
                     .Key; // Return the action (Reload, Shoot, etc.)
    }

    // Example method to use the decision tree for AI decision-making
    public void ChooseActionAI(DecisionTreeNode decisionTree, Player opponent)
    {
        UpdatePlayerAmmo();  // Update the player's ammo and cannon status

        // Create a GameState object based on the current game data
        GameState currentState = new GameState
        {
            PlayerAmmo = ammoCount,                   // Current ammo of the player
            OpponentAmmo = opponent.bullets,          // Ammo of the opponent
            PlayerAction = opponent.action,           // The opponent's last action (as a placeholder)
            PlayerHasCannon = hasCannon,              // Whether the player has a cannon (Bazuka)
            OpponentHasCannon = opponent.hasCannon   // Whether the opponent has a cannon
        };

        // Traverse the decision tree to choose the best action based on the current state
        action = TraverseTree(decisionTree, currentState);
    }

    // Traverse the decision tree and return the action based on the current game state
    private GameNamespace.Action TraverseTree(DecisionTreeNode node, GameState state)
    {
        // If this node is a leaf node, return the action at the leaf
        if (node.IsLeaf) return node.LeafAction.Value;

        // Get the feature value for this node (e.g., PlayerAmmo = 5)
        string featureValue = GetFeatureValue(state, node.Feature);

        // If the feature value exists in the child nodes, continue traversing
        if (node.Children.TryGetValue(featureValue, out DecisionTreeNode child))
        {
            return TraverseTree(child, state); // Continue traversing the tree down this path
        }

        // If no matching child node is found, return a default action (e.g., Block)
        return GameNamespace.Action.b;
    }
}
