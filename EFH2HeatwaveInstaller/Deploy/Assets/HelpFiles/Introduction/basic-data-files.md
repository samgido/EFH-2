## BASIC DATA FILES:
Required Files: 
```
COVER.txt
```

Optional Files:
```
SOILS.hg
```

COVER.txt - This file consists of the runoff curve number (CN) tables found in **NEH 630.09, Hydrologic Soil-Cover Complexes**.  These tables list CN by HSG, cover type, and hydrologic condition for various land use types.  The CN tables are used in computing a weighted CN for the watershed being evaluated.  The **COVER.TXT** file is required and must reside in the same folder as the program executable file (EFH2.EXE).  If the **COVER.TXT** file is not available, **EFH-2** computer program will not initialize and will not operate.

**SOILS.HG** – The Hydrologic Soil Group (HSG) lookup tool uses this file for populating the table. The file contains three columns, separated by the TAB character. The first column is the soil name. The second is the surface texture, useful when there are multiple textures for the same soil name. The third column is the hydrologic group. An easy way to create this file is to use a spreadsheet program, such as Excel. Fill in the three columns and save as a Text (Tab Delimited) file. Rename to **SOILS.HG** and store in the EFH2 program folder.


| Column | Maximum Number of Characters | Data Description |
| ------ | ---------------------------- | ---------------- |
| 1       |                          40    |              Soil map unit name    |
|  2      |                          10    | Textural class                 |
|   3     |                          10    |             Hydrologic soil group     |
