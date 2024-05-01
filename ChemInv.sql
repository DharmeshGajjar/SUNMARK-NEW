DECLARE @MscNm varchar(50), @MscCd varchar(30), @CmpCdN BigInt
DECLARE XCUR CURSOR FOR SELECT MscNm, MscCD, MscCmpCdn FROM BOM.DBO.MscMst WHERE MscTyp= 'PCK' ORDER BY MscVou
OPEN XCUR
FETCH NEXT FROM XCUR INTO @MscNm, @MscCd, @CmpCdN
WHILE (@@FETCH_STATUS=0)
BEGIN
	Insert Into MscMst (MscTyp, MscPos, MscNm, MscCD, MscActYN, MscUsrVou, MscCmpCdN, MscCliVou)
				Values ('APP', 0, @MscNm, (Case IsNull(@MscCd,'') When '' Then @MscNm Else @MscCd End), 'Yes', 1, @CmpCdN, 0)
	FETCH NEXT FROM XCUR INTO @MscNm, @MscCd, @CmpCdN
END
CLOSE XCUR
DEALLOCATE XCUR

Select MscTyp From MscMst Group By MscTyp