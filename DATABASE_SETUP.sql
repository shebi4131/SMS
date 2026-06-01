-- Database Setup & Configuration Guide
-- This file contains SQL commands to verify your database structure

-- ============================================
-- VERIFY TABLE STRUCTURE
-- ============================================

-- Check if tables exist
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
AND TABLE_NAME IN (
	'Tbl_Memberinfo',
	'Tbl_MembersReg',
	'Tbl_MemberInv',
	'Tbl_MInvPlotFlat',
	'Tbl_PlotSize'
);

-- ============================================
-- VERIFY COLUMNS IN EACH TABLE
-- ============================================

-- Check Tbl_Memberinfo columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tbl_Memberinfo'
ORDER BY ORDINAL_POSITION;

-- Check Tbl_MembersReg columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tbl_MembersReg'
ORDER BY ORDINAL_POSITION;

-- Check Tbl_MemberInv columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tbl_MemberInv'
ORDER BY ORDINAL_POSITION;

-- Check Tbl_MInvPlotFlat columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tbl_MInvPlotFlat'
ORDER BY ORDINAL_POSITION;

-- Check Tbl_PlotSize columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tbl_PlotSize'
ORDER BY ORDINAL_POSITION;

-- ============================================
-- EXPECTED COLUMN MAPPING
-- ============================================

/*
Tbl_Memberinfo Expected Columns:
- MemberinfoID (INT, Primary Key)
- FullName (VARCHAR/NVARCHAR)
- CNIC (VARCHAR, 13 digits)
- MeshipNo (VARCHAR, Membership Number)
- oldMNo (VARCHAR, Old Membership Number)
- Address1 (VARCHAR/NVARCHAR)
- CellNo (VARCHAR)

Tbl_MembersReg Expected Columns:
- MembersRegID (INT, Primary Key)
- MemberInvID (INT, Foreign Key to Tbl_MemberInv)
- MemberSID (INT, Foreign Key to Tbl_Memberinfo.MemberinfoID)

Tbl_MemberInv Expected Columns:
- MemberInvID (INT, Primary Key)
- ProID (VARCHAR, Project ID)
- AlotTransEntryDate (DATETIME)
- BookNo (VARCHAR, Book Number)

Tbl_MInvPlotFlat Expected Columns:
- MInvPlotFlatID (INT, Primary Key)
- MemberInvID (INT, Foreign Key to Tbl_MemberInv)
- PFlotNo (VARCHAR, Plot Number)
- PFlotNo2 (VARCHAR, Plot Number 2)
- StNo (VARCHAR, Street Number)
- PlotSizeID (INT, Foreign Key to Tbl_PlotSize)

Tbl_PlotSize Expected Columns:
- PlotSizeID (INT, Primary Key)
- PlotSize (VARCHAR) - Note: May be named differently in your DB
*/

-- ============================================
-- DATA VERIFICATION QUERY
-- ============================================

-- Run the application's main query to verify data
SELECT 
	Tbl_Memberinfo.FullName, 
	Tbl_Memberinfo.CNIC, 
	Tbl_MInvPlotFlat.PFlotNo, 
	Tbl_MInvPlotFlat.PFlotNo2, 
	Tbl_MInvPlotFlat.StNo,
	Tbl_PlotSize.PlotSize, 
	Tbl_MemberInv.ProID, 
	Tbl_Memberinfo.MeshipNo,
	Tbl_Memberinfo.oldMNo,
	Tbl_Memberinfo.Address1,
	Tbl_Memberinfo.CellNo,
	Tbl_MemberInv.AlotTransEntryDate,
	Tbl_MemberInv.BookNo
FROM Tbl_MInvPlotFlat 
INNER JOIN Tbl_MembersReg ON Tbl_MInvPlotFlat.MemberInvID = Tbl_MembersReg.MemberInvID 
INNER JOIN Tbl_Memberinfo ON Tbl_MembersReg.MemberSID = Tbl_Memberinfo.MemberinfoID 
INNER JOIN Tbl_MemberInv ON Tbl_MInvPlotFlat.MemberInvID = Tbl_MemberInv.MemberInvID 
INNER JOIN Tbl_PlotSize ON Tbl_MInvPlotFlat.PlotSizeID = Tbl_PlotSize.PlotSizeID 
ORDER BY Tbl_Memberinfo.FullName;

-- ============================================
-- COUNT RECORDS
-- ============================================

SELECT 
	'Tbl_Memberinfo' as TableName, COUNT(*) as RecordCount FROM Tbl_Memberinfo
UNION ALL
SELECT 'Tbl_MembersReg', COUNT(*) FROM Tbl_MembersReg
UNION ALL
SELECT 'Tbl_MemberInv', COUNT(*) FROM Tbl_MemberInv
UNION ALL
SELECT 'Tbl_MInvPlotFlat', COUNT(*) FROM Tbl_MInvPlotFlat
UNION ALL
SELECT 'Tbl_PlotSize', COUNT(*) FROM Tbl_PlotSize;

-- ============================================
-- SEARCH BY CNIC (TEST QUERY)
-- ============================================

-- Replace with one of your CNICs
DECLARE @TestCNIC VARCHAR(20) = '1730111913483';

SELECT 
	Tbl_Memberinfo.FullName, 
	Tbl_Memberinfo.CNIC, 
	Tbl_MInvPlotFlat.PFlotNo, 
	Tbl_MInvPlotFlat.PFlotNo2, 
	Tbl_MInvPlotFlat.StNo,
	Tbl_PlotSize.PlotSize, 
	Tbl_MemberInv.ProID, 
	Tbl_Memberinfo.MeshipNo,
	Tbl_Memberinfo.oldMNo,
	Tbl_Memberinfo.Address1,
	Tbl_Memberinfo.CellNo,
	Tbl_MemberInv.AlotTransEntryDate,
	Tbl_MemberInv.BookNo
FROM Tbl_MInvPlotFlat 
INNER JOIN Tbl_MembersReg ON Tbl_MInvPlotFlat.MemberInvID = Tbl_MembersReg.MemberInvID 
INNER JOIN Tbl_Memberinfo ON Tbl_MembersReg.MemberSID = Tbl_Memberinfo.MemberinfoID 
INNER JOIN Tbl_MemberInv ON Tbl_MInvPlotFlat.MemberInvID = Tbl_MemberInv.MemberInvID 
INNER JOIN Tbl_PlotSize ON Tbl_MInvPlotFlat.PlotSizeID = Tbl_PlotSize.PlotSizeID 
WHERE LTRIM(RTRIM(Tbl_Memberinfo.CNIC)) = @TestCNIC;

-- ============================================
-- IF COLUMN NAMES DON'T MATCH
-- ============================================

/*
If your column names are different, you need to update:

1. Models in SMS/Models/ directory
   - Update property names to match your database columns

2. ApplicationDbContext in SMS/Data/
   - Update the .Property() configurations

3. MemberService query in SMS/Services/
   - Update the SELECT mapping in LINQ query

Example: If your column is "PlotNumber" instead of "PFlotNo"
- Change Model property: public string PlotNumber { get; set; }
- Update DbContext: entity.Property(e => e.PlotNumber).HasMaxLength(50);
- Update MemberService: .Select(x => new MemberLookupResult { PFlotNo = x.MInvPlotFlat.PlotNumber, ... })
*/

-- ============================================
-- CREATE INDEXES (RECOMMENDED)
-- ============================================

-- Index on CNIC for faster lookups
CREATE NONCLUSTERED INDEX IX_Memberinfo_CNIC 
ON Tbl_Memberinfo(CNIC);

-- Index on foreign keys
CREATE NONCLUSTERED INDEX IX_MembersReg_MemberInvID 
ON Tbl_MembersReg(MemberInvID);

CREATE NONCLUSTERED INDEX IX_MembersReg_MemberSID 
ON Tbl_MembersReg(MemberSID);

CREATE NONCLUSTERED INDEX IX_MInvPlotFlat_MemberInvID 
ON Tbl_MInvPlotFlat(MemberInvID);

CREATE NONCLUSTERED INDEX IX_MInvPlotFlat_PlotSizeID 
ON Tbl_MInvPlotFlat(PlotSizeID);

-- ============================================
-- SAMPLE TEST DATA QUERY
-- ============================================

-- View 5 sample records with all details
SELECT TOP 5
	m.FullName,
	m.CNIC,
	mi.ProID,
	mpf.PFlotNo,
	ps.PlotSize,
	m.MeshipNo,
	m.Address1
FROM Tbl_Memberinfo m
INNER JOIN Tbl_MembersReg mr ON m.MemberinfoID = mr.MemberSID
INNER JOIN Tbl_MemberInv mi ON mr.MemberInvID = mi.MemberInvID
INNER JOIN Tbl_MInvPlotFlat mpf ON mi.MemberInvID = mpf.MemberInvID
INNER JOIN Tbl_PlotSize ps ON mpf.PlotSizeID = ps.PlotSizeID
ORDER BY m.FullName;
