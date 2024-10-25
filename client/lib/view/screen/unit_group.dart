import 'package:grammar_guide_client/model/provider/user_data_provider.dart';
import 'package:provider/provider.dart';

import '../../model/all.dart';
import 'routes.dart';
import 'util.dart';
import 'package:flutter/material.dart';

class UnitGroupPageData {
  final List<UnitGroup> groups;
  final int initiallySelected;
  final String title;
  final List<(String, String)> tabTitles;

  UnitGroupPageData(this.groups, this.initiallySelected, this.title)
      : tabTitles = groups.map((group) {
          final lst = group.title.split(":");
          return (lst.first.trim(), lst.last.trim());
        }).toList();
}

class UnitGroupPage extends StatefulWidget {
  const UnitGroupPage({super.key});

  @override
  State<UnitGroupPage> createState() => _UnitGroupPageState();
}

class _UnitGroupPageState extends State<UnitGroupPage> {
  @override
  Widget build(BuildContext context) {
    final data =
        ModalRoute.of(context)!.settings.arguments as UnitGroupPageData;
    return DefaultTabController(
      length: data.groups.length,
      initialIndex: data.initiallySelected,
      child: Scaffold(
        appBar: AppBar(
          backgroundColor: getAppBarBackgroundColor(context),
          title: Text(data.title),
          bottom: TabBar(
            tabAlignment: TabAlignment.center,
            isScrollable: true,
            tabs: data.tabTitles
                .map((group) => Tab(
                      icon: Text(group.$1),
                      child: Text(group.$2),
                    ))
                .toList(),
          ),
        ),
        body: TabBarView(
          children: data.groups
              .map((group) => buildUnitList(context, group.units))
              .toList(),
        ),
      ),
    );
  }
}

Widget buildUnitList(BuildContext context, List<Unit> units) =>
    ListView.builder(
      itemCount: units.length,
      itemBuilder: (BuildContext context, int index) {
        //todo get user id
        return _ListRow(units[index], "", "");
      },
    );

class _ListRow extends StatelessWidget {
  final String userId;
  final String guideId;
  final Unit unit;

  const _ListRow(this.unit, this.userId, this.guideId);

  @override
  Widget build(BuildContext context) {
    return ListTile(
      title: Text(
        'Unit ${unit.index}: ${unit.title}',
        style: const TextStyle(fontSize: 16.0),
        overflow: TextOverflow.ellipsis,
      ),
      trailing: Consumer<UserDataProvider>(
          builder: (context, dataProvider, child) => Checkbox(
                value: dataProvider.isCompleted(unit),
                onChanged: null,
              )),
      onTap: () => Navigator.pushNamed(
        context,
        Routes.unit,
        arguments: unit,
      ),
    );
  }
}
