DECLARE @LOTVOU BIGINT, @CMPVOU BIGINT, @COILNO VARCHAR(30), @MCOILNO VARCHAR(30), @SUPCOILNO VARCHAR(30)
DECLARE XCUR CURSOR FOR SELECT JOTCMPVOU, JOTVOU FROM JOBTRN WHERE JOTJOBVOU = 197 ORDER BY JotRecCoilNo
OPEN XCUR
FETCH NEXT FROM XCUR INTO @CMPVOU, @LOTVOU
WHILE (@@FETCH_STATUS=0)
BEGIN
	UPDATE JOBTRN SET JOTCMPVOU = @CMPVOU WHERE JOTVOU = @LOTVOU	
	FETCH NEXT FROM XCUR INTO @CMPVOU, @LOTVOU
END
CLOSE XCUR
DEALLOCATE XCUR
