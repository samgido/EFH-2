## Time of Concentration Field

#### **Time of Concentration**

This field will be calculated when any valid data exists in the Runoff Curve Number, Watershed Length and Watershed Slope fields. This field may be entered directly when the user has determined the time of concentration external to the program. Any subsequent changes to the Runoff Curve Number, Watershed Length or Watershed Slope fields will replace the user entered data with the new calculated value.

![](time-of-concentration-1.png)  
  
The Lag Equation is used to compute Tc within EFH-2:

where:

Tc        = time of concentration, hours

_l_         = flow length, feet

Y        = average watershed land slope, percent

S        = maximum potential retention, inches
![](time-of-concentration-2.png)

CN    = runoff curve number

More information on the derivation of the Lag Equation can be found in [NEH 630.15, Time of Concentration](https://directives.sc.egov.usda.gov/OpenNonWebContent.aspx?content=27002.wba).