select * from apoen

select a.id as novi_id_apoen, b.* from brojanje b left join apoen a on b.vrijednost = a.vrijednost

select * from brojanje

update brojanje
set id_apoen = (CASE
	WHEN id_apoen = 3 THEN 30
	WHEN id_apoen = 4 THEN 50
END) where id_apoen in (3, 4)