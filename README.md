# BioSNTR_Plot_Points

This project is for the SDSU REU BioSNTR 2018 Summer Research Program

The goal of this program is to create a tool for visualizing 3-Dimensional Principle Component Analysis (PCA) in virtual reality. Ideally, such a tool would be useful in gaining more understanding for various data. The primary interface used will be HTC Vive.

## Table of Contents

[Project Status](https://github.com/thunder2011/BioSNTR_Plot_Points#project-status)

[Code Style / Framework Used](https://github.com/thunder2011/BioSNTR_Plot_Points#code-style--framework-used)

[Intstructions on How to Use](https://github.com/thunder2011/BioSNTR_Plot_Points#how-to-use)

[Examples](https://github.com/thunder2011/BioSNTR_Plot_Points#example-usage)

[External References](https://github.com/thunder2011/BioSNTR_Plot_Points#external-references)

[Acknowledgements](https://github.com/thunder2011/BioSNTR_Plot_Points#acknowledgements)

## Project Status

This project is still in development. More specifically, the VR components have not been tested as no there is no appropriate hardware in my general vicinity. This should be resolved by 7/1/2018.

[Back to TOC](https://github.com/thunder2011/BioSNTR_Plot_Points#table-of-contents)

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

[Back to TOC](https://github.com/thunder2011/BioSNTR_Plot_Points#table-of-contents)

## How to Use

### Plotting the PCA in 3D
When the application first starts the user should see a main menu that looks something like the following: 

<img width="474" alt="vr app menu 1" src="https://user-images.githubusercontent.com/31462296/41296176-fbe6155c-6e21-11e8-9bbb-f27565770840.png">

In these instructions, I will go over the meaning of each of the possible inputs on the menu and how to properly format said inputs. 

1. Scale - Scale is how large or small one wants the subsequent plot to be. Larger scales results in more space between datapoints while smaller scales results in the opposite.
2. Directory - This is where one will input the absolute directory path that leads to the folder that contains the data file.
3. File Name - The name of the data file that contains the relevant data. IMPORTANT: the file name must include the file type (.txt, .csv, etc.)!
4. Flip Data - The data from the file is read in the following manner: the horizontal rows are the individual data points while the vertical columns are the various dimensions of the data. As a consequence, occasionally, the data in the file will be the transpose of what the user actually wants to plot. In such situations, click the flip data button so that it shows 'true'. 
5. Coor_Data - The application is not very powerful in computing the PCA and projecting the data into three-dimensions. For small inputs it will be fine, but in cases where the input file is too large or some other reason, the application will likely crash. In such situations, it would be better to first compute the PCA projection and coordinate data in some other software / language such as Python or R before importing that as the datafile. If this happens, click the coor_data button so that is shows the appropriate value.
6. More Options - Clicking this button will open a second menu with more inputs, including the button that will actually plot the PCA projection. This second menu should look like this:

<img width="475" alt="vr app menu 2" src="https://user-images.githubusercontent.com/31462296/41296220-179418c6-6e22-11e8-8a81-b5bb78491712.png">

7. Exclude Columns - Occasionally, there will be several columns of data that the user does not want to include in the graphical rendering or PCA computation. For example, for data files in the form of .csv or .txt, the user would not want to include the first column that contains the numbering of the rows of data. In these situations, list the various columns that should be excluded, using commas and spaces to separate the numbers (i.e. 0, 1, 2, 3, 4). The first column in the data is column 0, the second is 1, etc. IMPORTANT: If the data is flipped from the previous 'flip-data' functionality, the columns are in reference to the columns in the transpose of the data (i.e the rows in the original data). Columns headers are expected and inherently excluded from the PCA projection / graphical rendering.

8. Know Cat.? - If there is a column that contains categories / bins that individual data points can be placed into and the user has a desire to show these categories, then click this box to show the appropriate value.

9. Cat. Column - If the previous value is true, then input the column where the categorys are. If the previous value is false, this option will be grayed-out. IMPORTANT: Do not double input the category column into both the exclude column option and here. 

10. Calculate PCA - This graphically renders the data from the input file according to the previous inputs. Clicking on this button will move the user to a new scene where the previous options will not be available. To go back to this input scene, see User Interaction - Pause Menu.

11. Back - Return to the previous menu.

### User Interaction

When the VR implementation in HTC Vive is completely done, the following instructions should apply for User Interaction:

0. There should be a constant, active laser coming out of the front of the HTC Vive controller.

Image Here

1. Pause Menu - Hitting the application menu button should pause the application (one can no longer move around but should still be able to look around) and pop up a menu that has the options 'resume', 'change input', and 'quit'. The first resumes the application, the second moves back to the main menu scene for a new input, and the third quits the application. The user can select options using the laser and the hair trigger click.

Image Here

2. Movement - The user can use the touchpad to move in the horizontal plane. To move vertically, the user should press the grip buttons. The right grip button will move the user upwards while the left grip button will move the user downwards. Currently, movement is gradual and may induce motion-sickness, but a future version may have movements be in short bursts instead.

3. Selecting points to view - If the laser points to a data point, then the user can then hit the hair trigger to have the name of the data point show up in a visible text element.

[Back to TOC](https://github.com/thunder2011/BioSNTR_Plot_Points#table-of-contents)

## Example Usage

These examples will demonstrate what to input into the various inputfields and what the end result should look like. All example data files mentioned are located on my computer in the absolute directory path /Users/ericfeng/Desktop/BioSNTR. 

<img width="767" alt="example path 1" src="https://user-images.githubusercontent.com/31462296/41296942-ac7d670c-6e23-11e8-9d15-e82a1c6b5345.png">

### Example 1
In this example we will plotting data regarding irises from a datafile named [iris.csv](Assets/Resources/iris.csv).

I fill out the various inputs of the main menu accordingly: 

1. Scale - 10. Through experimentation, I have determined that a scale of 10 makes the graph look nice.
2. Directory - /Users/ericfeng/Desktop/BioSNTR
3. File Name - iris.csv
4. False. I do not want the transpose of the data to be plotted
5. False. iris.csv does not contain coordinate data

<img width="475" alt="example 1 menu 1" src="https://user-images.githubusercontent.com/31462296/41297577-0ce1a5ee-6e25-11e8-9f09-ec45a26021f6.png">

On the second page of the main menu (numbered according to instructions above): 

7. 0. I don't want to include the first column that simply numbers the rows of data.
8. True. I have a column that categorizes my various points.
9. 5. The column that contains the category names is column 5. Notice that I didn't double input the category column into the exclude columns list.

<img width="472" alt="example 1 menu 2" src="https://user-images.githubusercontent.com/31462296/41297926-daf103a8-6e25-11e8-9d40-26629ac26e94.png">

After inputting the values, I hit the calculate PCA button, and the application calculates and projects the data onto the first 3 principle components. Like so:

<img width="678" alt="example 1 plot" src="https://user-images.githubusercontent.com/31462296/41297618-241cb41a-6e25-11e8-9101-403de714ae07.png">

### Example 2
In this example we will be plotting coordinate data regarding mouse embryo development from [coord_data.csv](Assets/Resources/coord_data.csv). The original data file was too large, and I used numpy to calculate these points. Source:  Deng, Qiaolin, et al. “Single-Cell RNA-Seq Reveals Dynamic, Random Monoallelic Gene Expression in Mammalian Cells.” Science, vol. 343, no. 6167, 10 Jan. 2014, pp. 193 –196., doi:10.1126/science.1245316.

1. Scale - 30 
2. Directory - /Users/ericfeng/Desktop/BioSNTR
3. File Name - coord_data.csv
4. False 
5. True. coord_data.csv does contain coordinate data

<img width="474" alt="example 2 menu 1" src="https://user-images.githubusercontent.com/31462296/41298486-13438630-6e27-11e8-9e5f-da0fbdb79086.png">

7. 0. I don't want to include the first column that simply numbers the rows of data.
8. True 
9. 4 

<img width="476" alt="example 2 menu 2" src="https://user-images.githubusercontent.com/31462296/41298504-20b29450-6e27-11e8-97bf-3bac7c1d820f.png">

Final plot: 

<img width="665" alt="example 2 plot" src="https://user-images.githubusercontent.com/31462296/41298531-2d2ca0fe-6e27-11e8-8050-22c31b3aca5d.png">

### Example 3
In this example we will be plotting the transpose of the data in [Processed_Data.csv](Assets/Resources/Processed_Data.csv). Source: Tsai MH, Chen X, Chandramouli GV, Chen Y, Yan H, Zhao S, Keng P, Liber HL, Coleman CN, Mitchell JB, Chuang EY: Transcriptional responses to ionizing radiation reveal that p53R2 protects against radiation-induced mutagenesis in human lymphoblastoid cells. Oncogene 2006, 25:622-632.

1. Scale - 5 
2. Directory - /Users/ericfeng/Desktop/BioSNTR
3. File Name - Processed_Data.csv
4. True 
5. False 

<img width="475" alt="example 3 menu 1" src="https://user-images.githubusercontent.com/31462296/41299681-9df954a6-6e29-11e8-9129-e8ac7cbb42c9.png">

7. 0
8. False 
9. N/A

<img width="479" alt="example 3 menu 2" src="https://user-images.githubusercontent.com/31462296/41310712-e9763688-6e47-11e8-82a0-1ccc5b1af7bc.png">

Final plot:

<img width="661" alt="example 3 plot" src="https://user-images.githubusercontent.com/31462296/41310726-f3ee6c98-6e47-11e8-966b-26db245f0679.png">

[Back to TOC](https://github.com/thunder2011/BioSNTR_Plot_Points#table-of-contents)

## External References
The following are all third-parties that I have used code implementations from or referenced in the development of this application:
1. CSV-Reader from PrinzEugn
2. The basic data plotting functionality from Big Data Social Science Fellows @ Penn State. 
3. HTC Vive controller scripts from Eric Van de Kerckhove on raywenderlich.com
4. Reading Text Files from a directory outside Unity from Daniel Robledo on Unity Support
5. Accord Framework for PCA
6. Unity and Microsoft Scripting Documentation

[Back to TOC](https://github.com/thunder2011/BioSNTR_Plot_Points#table-of-contents)

### Acknowledgements
Special recognition to the following: 
1. Professor Xijin Ge from SDSU who acted as my advisor for the duration of the project
2. BioSNTR for funding the development of this application
3. UC Berkeley and Virtual Reality @ Berkeley for the use of their equipment in the development and testing of the VR components of the application

[Back to TOC](https://github.com/thunder2011/BioSNTR_Plot_Points#table-of-contents)
