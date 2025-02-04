## Glossary

Data entry field definitions, sorted by the following screens:

#### **Basic data**

|**Field**|**Definition**|
|---|---|
|**By**|Initials or name of the person making the hydrology determination.|
|**Client**|The name of the producer, participant or landowner for the project.|
|**County**|Name of the County where the project is located. May be typed in directly, but should be selected from the dropdown list after the state has been entered.|
|**Date**|Automatically opens with the system (todays) date. May be entered manually.|
|**Drainage Area**|The area in acres within the watershed boundary contributing to the project site. Allowable range is 1 to 2000 acres.|
|**Practice**|Name or description of the practice for the project.|
|**Runoff Curve Number**|Weighted NRCS curve number representing the portion of precipitation that runs off the land. Allowable range is 40 to 98.|
|**State**|Two letter postal code representing the state may be typed in directly or selected from the dropdown list where the project is located. Requires two letters.|
|**Time of Concentration**|The time in hours for runoff to flow from the most hydraulically remote point to the project site. Calculated by the program, but may be manually calculated from the sum of the projected flow velocities of each reach along the flow path. Allowable range is 0.1 to 10 hours. Calculated values outside that range are reset to the nearest range limit.|
|**Watershed Length**|Length in feet along the flow path from the hydraulically most distant point to point of interest. Allowable range is 200 to 26000.|
|**Watershed Slope**|Average slope in percent of the all the contributing land within the watershed boundary. Allowable range is 0.5 to 64. Slopes from 0.1 to 0.49 may be used, however, warning messages will be generated and the user should be very cautious with its use.|

#### **Rainfall/Discharge data**

|**Field**|**Definition**|
|---|---|
|**Rainfall Distribution**|Rainfall distributions used by SCS/NRCS are generally 24 hours in duration using NOAA Atlas 14, NOAA Atlas 2 and TP-40 data.|
|**Dimensionless Unit Hydrograph**|The standard SCS dimensionless unit hydrograph can be transformed into discharge versus time hydrograph (UH) for any watershed, given drainage area, time of concentration, and peak rate factor. The standard dimensionless unit hydrograph is 484 <standard>.|
|**Frequency**|Recurrence interval in years for the storms.|
|**Peak Flow**|Maximum discharge in cubic feet per second for the runoff resulting from the defined storm.|
|**Runoff**|The volume of the excess precipitation, in inches on the watershed, not absorbed by the soil nor held on the vegetation.|
|**Select Hydrograph**|The single or multiple hydrographs can be plots to view.|

#### **Average Slope Calculator**

|**Field**|**Definition**|
|---|---|
|**Average slope**|The calculated average slope in percent of all the contributing land within the watershed boundary. It is the product of the length of contours times the contour interval divided by the drainage area.|
|**Contour interval**|The difference in elevation between two adjacent contours. This must be the same interval for all contour lengths measured.|
|**Drainage Area**|The area in acres within the watershed boundary contributing to the project site.|
|**Length of Contours**|The sum of the lengths of all the contours within the watershed.|