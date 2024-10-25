import 'package:flutter/material.dart';
import 'package:grammar_guide_client/model/provider/user_data_provider.dart';
import 'package:grammar_guide_client/view/screen/unit_group.dart';
import 'package:grammar_guide_client/view/screen/util.dart';
import 'package:provider/provider.dart';

class BookmarksScreen extends StatelessWidget {
  const BookmarksScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<UserDataProvider>(
      builder: (context, value, child) => Scaffold(
        appBar: AppBar(
            backgroundColor: getAppBarBackgroundColor(context),
            title: const Text("Bookmarks"),
            bottom: PreferredSize(
              preferredSize: const Size.fromHeight(10),
              child: value.load
                  ? const LinearProgressIndicator()
                  : const SizedBox.shrink(),
            )),
        body: value.load
            ? const SizedBox.shrink()
            : buildUnitList(context, value.units.toList()),
      ),
    );
  }
}
