## Menu "Tools"

#### **Average Slope Calculator**
![](tools-asc.png)

This command is available only when the ‘Basic Data’ tab is activated. Use this command to open the average slope calculator dialog window. This window allows the user to determine the average watershed slope using the sum of the length of all the contours within the drainage area boundary and the associated contour interval. This calculation uses the following formula:

![](tools-formula.png)

|Where:|Y =|Average watershed slope in percent,|
|---|---|---|
||C =|Total contour length in feet,|
||I =|Contour interval in feet, and|
||A =|Drainage area in acres.|

Upon exiting this window, the values for average slope and drainage area will be transferred back to the ‘Basic Data’ entry fields. It should be noted that a drainage area larger than 2000 acres may be used in this window. However, this drainage area will not be transferred back.

#### **Hydrologic Soil Group**
![](tools-hsg.png)

This command is available only when the ‘RCN’ tab is activated. Use this command to open the hydrologic soil group lookup dialog window. This window allows the user to view the data in the [SOILS.HG](Introduction\basic-data-files.md) file to lookup the hydrologic group value, A, B, C, or D for various soil types.