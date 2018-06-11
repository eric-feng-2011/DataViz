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
    //N is the number of data points/entrys
    pca.Compute();
    
    //Transforms the initial data by projecting it into the third dimension
    //using the found principle component axises
    double[][] result = pca.Transform (inputMatrix, 3);
    
    return result;

## How to Use

## Example Usage

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
