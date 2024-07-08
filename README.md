# Stratery game prototype
My first attempt to make a game using a game engine - Unity, to be precise.

# Implemented features (Uncomplete, may contain bugs):
1) Economy, including Warfare part, and Logistics-making simulation
2) Interface using Unity UI
3) In-built world generator, reused the idea and a lot of code from my GameMapGenerator-CSharp project available here on GitHub
4) For world generation, the algorithms used are: Perlin noise, Diamond-Square, cellular automaton, Gauss smoothing, Flood-fill
5) For pathway building, Bresenham's line algorithm is used
6) For logistic path forming, Dijkstra algorithm is used
7) The user can control the country they choose after generation
8) The generated maps can be switched through the special Dropdown on the Main game panel
9) Every change made to the Economic, Logistic or Warfare object is displayed either on the map (RawImage) or inside the Informational ScrollView (left side)
10) The control is implemented through Action ScrollView (right side) and multiple panels to detect the action and react to it
11) The user can launch a destructful impact which ruins the economical, warfare and logistics objects
12) The user can put pressure on battle positions, decreasing the state of warfare units there
13) The user can check their current progress on Resources panel

# Economy features
1) Multiple industries using different resources to progress
2) Three levels of industries, with higher levels depending on lower ones
3) Creating a facility for an industry and launching a production increases the industry's progression
4) Each production can be increased and decreased, producing more or less products
5) Each production takes time to complete
6) One facility can contain multiple different productions
7) The user can change price of each product
8) There are storages which can contain limited amount of products
9) There are cities with no current functionality - except for them being the logistic nodes

# Logistics features
1) Economy and Warfare objects are the nodes which can be connected with...
2) ...Straight pathways (roads) or...
3) ...Non-straight paths through other nodes
4) These objects form the logistic graph
5) The paths formed can be displayed on the map one at a time - or hidden if "None" option is selected
6) If an impact has cut away a part of the path - it will try to rebuild itself using the remaining parts of the graph

# Warfare features
1) Warfare itself is a part of the Economy and has 4 its own industries
2) These industries's products are warfare resources
3) Each warfare resource has a state which can be dropped under battle pressure and restored in hospitals or workshops
4) Soldiers are also warfare resources. They can be trained on training grounds, which takes time
5) Warfare resources are contained separately in an Object Bank and are displayed when a "Check warfare resources" button is pressed on Resources panel
6) Some units depend on other units to be trained or transfered (WIP)
7) The units can be transfered to and from battle positions
8) The units in full-state can be removed from where they are and placed into Object Bank for further use

# Planned features (in further versions)
1) The diplomacy system, including reputation and unions
2) The trading between countries system
3) The "international" logistics system
4) Automated development of other countries with the simple AI ->
5) -> The more complex diplomacy, trading and warfare system
6) The social system, including public opinion on the decisions

# Illustrations
Generator start page:
![Screenshot_16](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/0614758b-4b37-4564-ba43-936c730bfb8e)

Prepare for start panel example:
![Screenshot_19](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/c637bb2e-7963-4e44-925c-b05286cdd040)

Countries legend:
![Screenshot_38](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/aaecce27-5432-44f1-90cd-a8c7431628f5)

Main panel with one selected point:
![Screenshot_17](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/7407e644-cf77-4667-987e-9f57bf241d41)

Resource panel AKA Information page:
![Screenshot_31](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/835f0472-40df-4265-90e1-b0cff2b156d1)

Facility edit example:
![Screenshot_20](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/af8b1faf-cf24-40af-8c2a-7e109d2636ab)

Error example:
![Screenshot_25](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/64648edf-1980-489c-b094-4ae3dedef1db)

Close-up of a logistic graph with a selected path:
![Screenshot_8](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/72b6451c-cf7e-484b-8fe5-730266932ad5)

The same graph, damaged with an impact, the path is rebuilt:
![Screenshot_14](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/20a21d31-6fa4-4140-ac8a-e5d4a0b7684d)

Battle position edit example:
![Screenshot_34](https://github.com/MarinaNikolaieva/StrategyGamePrototype-Ver1/assets/60624855/ae41841c-929a-4eff-90a9-0cfe47362160)
