# 🧪 Testing & Sample Data Guide

## Test CNICs from Your Original Query

You provided these CNICs in your original query. Use any of these to test:

### Image 1 - Haji Muhammad Family
```
1730111913483  (Haji Muhammad - Self)
1730113646117  (Khanzada - Father)
2120357092164  (Roshan Zari - Sister)
1730142993057  (Mumtaz Khan - Brother)
1730113605951  (Ali Muhammad - Brother)
2140640108134  (Gul Meena Bibi - Sister)
2140666140556  (Dilfroza - Sister)
```

### Image 1 - Basweer Khan Family
```
2140588889589  (Basweer Khan - Self)
2140522952863  (Shahzad Gul - Father)
1730151071460  (Shamala Bibi - Sister)
2140591227917  (Miraj - Brother)
```

### Image 2 - Standalone
```
1610131056155
```

### Image 3 - Muhammad Abbass Family
```
2110638578035  (Wali Khan - Father)
2110698728858  (Zarmina Bibi - Mother)
2110694244229  (Muhammad Abbass - Self)
```

### Image 3 - Zulqarnain Family
```
2130198523101  (Muhammad Karim - Father)
2130144182140  (Bibi Jamala - Mother)
2130189090583  (Zulqarnain - Self)
2130113981445  (Muhammad Saleem - Brother)
```

### Image 3 - Ijaz Ahmed Family
```
6110126259339  (Ijaz Ahmed - Self)
1410174862240  (Bibi Salma - Mother)
9040601513269  (Mubassar Ahmed - Brother)
1410133009890  (Afsa Bibi - Wife)
1410106880042  (Samia Bibi - Sister)
1410106774592  (Rozina Bibi - Sister)
```

### Image 3 - Firdous Khan Family
```
1410193278419  (Firdous Khan)
9040201177647  (Mustir Khan - Father)
1410101524296  (Samida Jan - Mother)
```

### Image 4 - Sohail Ahmed Family
```
6110140425777  (Sohail Ahmed - Self)
6110118739517  (Nisar Ahmed - Father)
6110118063600  (Shahnaz Bibi - Mother)
6110122628769  (Bilal Ahmed - Brother)
6110157844984  (Sobia Bibi - Sister)
6110182211752  (Nida Nisar - Sister)
```

## 🧪 Testing Steps

### 1. Create Test PDF
```
Steps to create a test PDF:
1. Open Word or Notepad
2. Type: 1730111913483
3. Save as PDF
4. Or use online text-to-PDF converter
```

### 2. Upload & Test
1. Open application at `https://localhost:5001`
2. Click upload area
3. Select test PDF
4. Verify CNIC is extracted
5. Click "Search Members"
6. Check if results appear

### 3. Verify Results
- Full Name should match database
- CNIC should be correct
- Plot information should display
- Click "View" to see full details

## 🔍 Database Verification

Before testing the app, verify your data:

```sql
-- Check if any test CNICs exist
SELECT * FROM Tbl_Memberinfo 
WHERE CNIC IN (
	'1730111913483',
	'2140588889589',
	'1610131056155',
	'2110694244229'
);

-- Should return member information
```

## 📝 Create Multiple Test PDFs

**Test 1 - Single CNIC:**
```
Create PDF with:
1730111913483
```

**Test 2 - Multiple CNICs:**
```
Create PDF with:
1730111913483
2140588889589
1610131056155
2110694244229
```

**Test 3 - Family Group:**
```
Create PDF with:
6110140425777
6110118739517
6110118063600
6110122628769
6110157844984
6110182211752
```

## ✅ Expected Results

When you search with a valid CNIC, you should see:

| Field | Example Value |
|-------|---------------|
| Full Name | Haji Muhammad |
| CNIC | 1730111913483 |
| Membership No | M-1234 |
| Plot No | P-123 |
| Plot Size | 500 |
| Street No | ST-5 |
| Cell No | 300-1234567 |
| Address | House 123, Street 5 |
| Project ID | PRJ-001 |
| Book No | BOOK-123 |
| Allocation Date | 15/06/2023 |

## 🐛 If No Results

**Possible Issues:**
1. CNIC doesn't exist in database
2. Database connection issue
3. Data relationship problem
4. CNIC has extra spaces

**Fix:**
```sql
-- Check raw data
SELECT 
	FullName, 
	CNIC, 
	LEN(CNIC) as CNICLength
FROM Tbl_Memberinfo 
ORDER BY FullName;

-- Look for spaces
SELECT 
	FullName, 
	'[' + CNIC + ']' as CNICWithBrackets
FROM Tbl_Memberinfo 
LIMIT 10;
```

## 🎯 Testing Scenarios

### ✅ Happy Path
1. Upload valid PDF
2. CNIC extracts correctly
3. Search finds member
4. Results display
5. View details modal opens

### ✅ Error Handling
1. Upload invalid file → Error message
2. No CNIC in PDF → Info message
3. CNIC not in DB → "No results found"
4. Database error → Error message displayed

### ✅ UI Functionality
1. Remove CNIC from list → Works
2. Search with no CNICs → Error
3. Click View → Modal opens
4. Close modal → Returns to results
5. Upload new file → Works

## 📊 Sample Test Data SQL

Run this to see what data should be returned:

```sql
-- Full test query with one CNIC
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
WHERE LTRIM(RTRIM(Tbl_Memberinfo.CNIC)) = '1730111913483';
```

## 🎊 Successful Test Indicators

✅ Application loads at https://localhost:5001  
✅ Upload area is visible and interactive  
✅ PDF upload works  
✅ CNIC extraction displays results  
✅ Search button triggers database query  
✅ Results table populates with data  
✅ View button opens modal  
✅ Modal displays complete information  
✅ No JavaScript errors in console (F12)  
✅ Connection string works  

## 📞 Troubleshooting Tests

If tests fail:

1. **Check console (F12)** for JavaScript errors
2. **Check Visual Studio output** for build errors
3. **Run test SQL query** to verify data exists
4. **Test connection string** separately
5. **Check file permissions** for PDF
6. **Verify CNIC format** (13 digits exactly)

---

**Ready to test? Follow the Testing Steps above!**
