# Roc.Data
Roc.Data 默认使用约定胜于配置的规则运行的

默认遵循以下约定

1 默认使用SQL2005以上数据库
2 默认实体名和数据库表名一致
3 默认实体属性和数据库表中列名一致

但是 , Roc.Data 也支持配置

1 切换数据库
	生成SQL语句 请使用 SqlLam<Area> sql = new SqlLam<Area>("u", ProviderType.MySql); 构造函数切换
	或者使用 sql.Type = ProviderType.Oracle; 设置属性切换

	执行数据库操作的时候 请使用 GlobalConfig.UseDb(ProviderType.Oracle); 切换

	生成SQL语句和执行数据库操作 没有联系,所有两者没有联系;

2 配置表名
	第一个方式是在实体上设置特性 如
	[Table("T_Area")]
    public class Area

	第二个方式是全局设置 , 全局可以将多个实体在一起设置 方式如下
	 SqlTableEntity areaTable = new SqlTableEntity(typeof(Area), "T_Area");
     GlobalConfig.Tables.Add(areaTable);

	 我们推荐第二个方式进行配置，但是，两个方式可以配合使用，优先使用第二个方式（即对一个实体同时配置了两个方式，会使用第二个方式生成SQL语句）

3 配置列名
	第一个方式是在实体的属性上设置特性 如
	[Column("C_AreaId")]
    public string AreaId { get; set; }

	第二个方式是全局设置 , 全局可以将多个实体并且其属性在一起设置 方式如下
	 SqlTableEntity areaTable = new SqlTableEntity(typeof(Area), "T_Area");
	 areaTable.Columns.Add(new SqlColumnEntity("AreaId", "C_AreaId"));
     GlobalConfig.Tables.Add(areaTable);

	 我们推荐第二个方式进行配置，但是，两个方式可以配合使用，优先使用第二个方式（即对一个实体的同一个属性同时配置了两个方式，会使用第二个方式生成SQL语句）

	 另外，还支持列名的 其他配置
	 主键配置 
			在列名上 增加 
			[Key]
			public string AreaId { get; set; }

			或者设置 SqlColumnEntity 的 Key 属性 为 true , 则认为主键配置生效
	 自增配置
			在列名上 增加 
			[Key(true)]
			public string AreaId { get; set; }

			或者设置 SqlColumnEntity 的 Increment 属性 为 true , 则认为自增配置生效
     忽略配置
		    在列名上 增加 
			[Ignore]
			public string AreaId { get; set; }

			或者设置 SqlColumnEntity 的 Ignore 属性 为 true , 则认为忽略配置生效 ，生效后 insert , update 操作会忽略该字段
	  读写配置
	        在列名上 增加 
			[Action(type: ActionType.Read)
			public string AreaId { get; set; }

			或者设置 SqlColumnEntity 的 ActionType 属性 , 则认为读写配置生效 ，分为 读操作，写操作，可读可写操作，不可操作
