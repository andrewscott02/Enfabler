This is the readme file for the Buddy-NPC-Dissertation Project

Behaviour Tree Tutorial was used for the basic setup of the behaviour tree
Link: https://www.youtube.com/watch?v=aR6wt5BlE-E

AI Info:

The "Assets/Scripts/AI" folder contains all of the scripts used for the behaviour trees, nodes, tasks, agent modelling and other AI related scripts

The behaviour trees for the AI agents are in the "Assets/Scripts/AI/BehaviourTree/BehaviourTrees" folder
The behaviour trees for the tested agents used in the experiment are the AdaptiveBehaviourTree (AM companion agent) and the IntervalBehaviourTree (IT companion agent)
The BasicBehaviourTree was used for the enemies and the FollowBehaviourTree was used for the companion AI in the tutorial scene

Only the behaviour tree system (including the BehaviourTree, Node, Selector and Sequence classes) were made using the Behaviour Tree Tutorial linked above
All other scripts were made by me

The BaseBehaviours script contains useful selectors and sequences that get used multiple times in all the agents
These base behaviours were designed to be generic and reusable between agents and make the behaviour trees tidier

The ConstructPlayerModel class is the script that analyses the player actions and constructs a model based on their playstyle
The AdaptiveBehaviourTree then uses this model to determine its actions

Experiment Info:

The "Assets/Scripts/Experiment" folder contains all of the scripts used to manage the experiment, including loading the correct scenes, managing the tutorial and generating the reference codes
The "Assets/Scenes/Experiment" folder contains all of the scenes that appear in the experiment
All of the other scenes in the "Assets/Scenes" folder are test scenes that were used to test combat mechanics or specific agent types

Quality Assurance Info:

The "Assets/UnitTests" folder contains all of the unit tests used for quality assurance
Evidence of testing is in the "Assets/Testing" folder