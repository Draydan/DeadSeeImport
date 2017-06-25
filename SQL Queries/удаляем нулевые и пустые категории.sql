-- удаляем нулевые и пустые категории

select * from Categories
where title is null or titlerus is null

select * from LinkProductWithCategories li
inner join Products p on p.id = li.product_ID
where category_ID is null

--delete from LinkProductWithCategories where
--category_ID in (
--select id from Categories where title is null)
--delete from Categories where title is null

--delete from LinkProductWithCategories where
--category_ID in (
--select id from Categories where titleRus is null)
--delete from Categories where titlerus is null


delete from LinkProductWithCategories where category_ID is null