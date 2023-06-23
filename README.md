# RaylibPerformanceEvaluation
A simple project to test the rendering and animation capabilities of Raylib with the C# bindings.

It is fairly simple to set up a test. You have to put Config objects in the testConfigs list, you can do this individually in the list declaration or with a loop in the CreateTestConfigs() function.

You can also change the row and column variables of the output table by changing the Logger.Save() function call at the end of the main game loop.

The output is recorded into a logs.txt file, the contetns of which can be directly pasted into Excel.

This repository does not contain the resources folder which holds all the models and textures.
