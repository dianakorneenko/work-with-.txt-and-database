use task1db;

select * from my_row;


GO
CREATE PROCEDURE give_sum_int AS
BEGIN
	select sum(int_str) as sum_int
	from my_row
END;

EXEC give_sum_int;

GO
CREATE PROCEDURE give_median_double AS
BEGIN
	select (
	(select max(double_str) as max_num
	from 
	(select top 50 percent double_str 
	from my_row 
	order by double_str
	) as maxtb)
	+
	(select min(double_str) as max_num
	from 
	(select top 50 percent double_str 
	from my_row 
	order by double_str DESC
	) as maxtb)
	)/2 as median_double
END;

EXEC give_median_double;

select sum(int_str) as max_int,(
select (
(select max(double_str) as max_num
from 
(select top 50 percent double_str 
from my_row 
order by double_str
) as maxtb)
+
(select min(double_str) as max_num
from 
(select top 50 percent double_str 
from my_row 
order by double_str DESC
) as maxtb)
)/2) as median_double
from my_row

GO
CREATE PROCEDURE give_median_double_and_sum_int AS
BEGIN
	select sum(int_str) as max_int,(
	select (
	(select max(double_str) as max_num
	from 
	(select top 50 percent double_str 
	from my_row 
	order by double_str
	) as maxtb)
	+
	(select min(double_str) as max_num
	from 
	(select top 50 percent double_str 
	from my_row 
	order by double_str DESC
	) as maxtb)
	)/2) as median_double
	from my_row
END;

EXEC give_median_double_and_sum_int;
