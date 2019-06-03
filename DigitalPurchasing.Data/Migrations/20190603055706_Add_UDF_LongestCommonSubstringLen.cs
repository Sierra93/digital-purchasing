using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Add_UDF_LongestCommonSubstringLen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) =>
            // From https://www.red-gate.com/simple-talk/blogs/string-comparisons-in-sql-the-longest-common-substring/
            migrationBuilder.Sql(@"
--This will need a NUMBERS table, stocked with numbers. If you haven't got one
--this will create it automatically 
IF NOT EXISTS (SELECT 1 FROM information_Schema.Tables
  WHERE table_name='Numbers')
  BEGIN 
    CREATE TABLE [dbo].[Numbers]
        (
         [number] [int],
         CONSTRAINT [Index_Numbers] PRIMARY KEY CLUSTERED ([number] ASC)
            ON [PRIMARY]
        )
    ON  [PRIMARY] 
  END
IF NOT EXISTS(SELECT 1 FROM numbers WHERE number=99999)
BEGIN
TRUNCATE TABLE numbers
    ;WITH Digits(i) AS 
      (SELECT i
       FROM (VALUES (1), (2), (3), (4), (5), (6), (7), (8), (9), (0)) AS X(i))
     INSERT INTO numbers(number)
       SELECT (D6.i*1000000 +D5.i*100000 + D4.i*10000 + D3.i * 1000 + D2.i * 100 
                   + D1.i * 10 + D0.i + 1) AS seq
        FROM Digits AS D0, Digits AS D1, Digits AS D2, Digits AS D3, 
           Digits AS D4, Digits AS D5, Digits AS D6
    END
GO

CREATE FUNCTION udf_LongestCommonSubstringLen
/**
summary:   >
 The longest common subSubstring (LCS) tells you the longest common substring between two strings.
	 If you, for example, were to compare 'And the Dish ran away with the Spoon' with 'away', you'd 
	 get 'away' as being the string in common. Likewise, comparing '465932859472109683472' with 
	 '697834859472135348' would give you '8594721'. This returns length of this substring
**/    
(
@firstString NVARCHAR(MAX),
@SecondString NVARCHAR(MAX)
)
RETURNS INT AS
BEGIN
DECLARE @Order INT, @TheGroup INT, @Sequential INT, @maxLen INT

--this table is used to do a quirky update to enable a grouping only on sequential characters
DECLARE  @Scratch TABLE (TheRightOrder INT IDENTITY PRIMARY KEY,TheGroup smallint, Sequential INT,
        FirstOrder smallint, SecondOrder smallint, ch CHAR(1))
--first we reduce the amount of data to those characters in the first string that have a match 
--in the second, and where they were.       
INSERT INTO @Scratch ( TheGroup , firstorder,  secondorder, ch)
   SELECT 
	Thefirst.number - TheSecond.number AS TheGroup,
	Thefirst.number, TheSecond.number, 
	TheSecond.ch 
   FROM --divide up the first string into a table of characters/sequence
    (SELECT number, SUBSTRING(@FirstString,number,1) AS ch
       FROM dbo.Numbers WHERE number <= LEN(@FirstString)) TheFirst 
   INNER JOIN --divide up the second string into a table of characters/sequence
    (SELECT number, SUBSTRING(@SecondString,number,1) AS ch
       FROM dbo.Numbers WHERE number <= LEN(@SecondString)) TheSecond
   ON Thefirst.ch = Thesecond.ch --do all valid matches
   ORDER BY Thefirst.number-TheSecond.number, TheSecond.number
--now @scratch has all matches in the correct order for checking unbroken sequence   
SELECT @Order=-1, @TheGroup=-1, @Sequential=0 --initialise everything
UPDATE @Scratch --now check by incrementing a value every time a sequence is broken
  SET @Sequential=Sequential = 
         CASE --if it is not a sequence from the one before increment the variable
           WHEN secondorder = @order + 1 AND TheGroup=@TheGroup 
           THEN @Sequential ELSE @Sequential+1 END,
   @Order=secondorder, 
   @TheGroup=TheGroup

SELECT top(1) @maxLen = COUNT(*)
  FROM @scratch GROUP BY TheGroup,Sequential 
  ORDER BY COUNT(*) DESC, MIN(firstOrder) ASC, MIN(SecondOrder) ASC
RETURN @maxLen
END
");

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
