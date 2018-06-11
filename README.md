# BioSNTR_Plot_Points

This project is for the SDSU REU BioSNTR 2018 Summer Research Program

The goal of this program is to create a tool for visualizing 3-Dimensional Principle Component Analysis (PCA) in virtual reality. Ideally, such a tool would be useful in gaining more understanding for various data. The primary interface used will be HTC Vive.

## Project Status

This project is still in development. More specifically, the VR components have not been tested as no there is no appropriate hardware in my general vicinity. This should be resolved by 7/1/2018.

## Code Style / Framework Used

This project was built using Unity3D, and the scripting was all done in C#.

The PCA implementations are from Accord Version 3.0.2; the reason why I chose to not use more modern versions of the Accord Framework was I did not know if newer versions would still be compatible with Unity 2017.3 (the version of Unity used in this project).

    
    PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(inputMatrix, AnalysisMethod.Center);
    
    //Computes N number of Principal components
    //N is the dimensionality of the data points/entrys
    pca.Compute();
    
    //Transforms the initial data by projecting it into three dimensions
    //using the found principle component axises
    double[][] result = pca.Transform (inputMatrix, 3);
    
    return result;

## How to Use

### Plotting the PCA in 3D
When the application first starts the user should see a main menu that looks something like the following: 

Image Here.

In these instructions, I will go over the meaning of each of the possible inputs on the menu and how to properly format said inputs. 

1. Scale - Scale is how large or small one wants the subsequent plot to be. Larger scales results in more space between datapoints while smaller scales results in the opposite.
2. Directory - This is where one will input the absolute directory path that leads to the folder that contains the data file.
3. File Name - The name of the data file that contains the relevant data. IMPORTANT: the file name must include the file type (.txt, .csv, etc.)!
4. Flip Data - The data from the file is read in the following manner: the horizontal rows are the individual data points while the vertical columns are the various dimensions of the data. As a consequence, occasionally, the data in the file will be the transpose of what the user actually wants to plot. In such situations, click the flip data button so that it shows 'true'. 
5. Coor_Data - The application is not very powerful in computing the PCA and projecting the data into three-dimensions. For small inputs it will be fine, but in cases where the input file is too large or some other reason, the application will likely crash. In such situations, it would be better to first compute the PCA projection and coordinate data in some other software / language such as Python or R before importing that as the datafile. If this happens, click the coor_data button so that is shows the appropriate value.
6. More Options - Clicking this button will open a second menu with more inputs, including the button that will actually plot the PCA projection. This second menu should look like this:

Image Here.

7. Exclude Columns - Occasionally, there will be several columns of data that the user does not want to include in the graphical rendering or PCA computation. For example, for data files in the form of .csv or .txt, the user would not want to include the first column that contains the numbering of the rows of data. In these situations, list the various columns that should be excluded, using commas and spaces to separate the numbers (i.e. 0, 1, 2, 3, 4). The first column in the data is column 0, the second is 1, etc. IMPORTANT: If the data is flipped from the previous 'flip-data' functionality, the columns are in reference to the columns in the transpose of the data (i.e the rows in the original data). Columns headers are expected and inherently excluded from the PCA projection / graphical rendering.

8. Know Cat.? - If there is a column that contains categories / bins that individual data points can be placed into and the user has a desire to show these categories, then click this box to show the appropriate value.

9. Cat. Column - If the previous value is true, then input the column where the categorys are. If the previous value is false, this option will be grayed-out. IMPORTANT: Do not double input the category column into both the exclude column option and here. 

10. Calculate PCA - This graphically renders the data from the input file according to the previous inputs. Clicking on this button will move the user to a new scene where the previous options will not be available. To go back to this input scene, see User Interaction - Pause Menu.

11. Back - Return to the previous menu.

### User Interaction

When the VR implementation in HTC Vive is completely done, the following instructions should apply for User Interaction:

0. There should be a constant, active laser coming out of the front of the HTC Vive controller.

1. Pause Menu - Hitting the application menu button should pause the application (one can no longer move around but should still be able to look around) and pop up a menu that has the options 'resume', 'change input', and 'quit'. The first resumes the application, the second moves back to the main menu scene for a new input, and the third quits the application. The user can select options using the laser and the touchpad click.

2. Movement / Teleportation - If the laser points to a location that the player can teleport to, then there will be some sort of indication. Afterwards, hitting the hair trigger will teleport the player to that location.

3. Selecting points to view - If the laser points to a data point, then the user can then hit the hair trigger to have the name of the data point show up in a GUI.

## Example Usage

In this example we will plotting data regarding irises from a datafile named 'iris.csv' that is itself located in the absolute directory path /Users/ericfeng/Desktop/BioSNTR. 

## External References
The following are all third-parties that I have used code implementations from or referenced in the development of this application:
1. CSV-Reader from PrinzEugn
2. The basic data plotting functionality from Big Data Social Science Fellows @ Penn State. 
3. HTC Vive controller scripts from Eric Van de Kerckhove on raywenderlich.com
4. Reading Text Files from a directory outside Unity from Daniel Robledo on Unity Support
5. Accord Framework for PCA
6. Unity and Microsoft Scripting Documentation

### Acknowledgements
Special recognition to the following: 
1. Professor Xijin Ge from SDSU who acted as my advisor for the duration of the project
2. BioSNTR for funding the development of this application
3. UC Berkeley and Virtual Reality @ Berkeley for the use of their equipment in the development and testing of the VR components of the application
