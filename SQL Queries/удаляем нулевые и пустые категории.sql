-- удаляем нулевые и пустые категории

select * from Categories

--delete from LinkProductWithCategories where
--category_ID in (
--select id from Categories where title is null)
--delete from Categories where title is null


--delete from LinkProductWithCategories where
--category_ID in (
--select id from Categories where titleRus is null)
--delete from Categories where titlerus is null