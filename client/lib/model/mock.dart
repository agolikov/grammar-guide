import 'all.dart';
//
// List<UnitGroup> generateTopics() {
//   return [
//     "Rodzajnik i Deklinacja Rzeczowników",
//     "Koniugacja Czasowników: Czas Teraźniejszy",
//     "Zgody Przymiotników",
//     "Formy Mnogie Rzeczowników",
//     "Przypadki w Gramatyce Polskiej",
//     "Czasy Przeszłe Czasowników: Dokonane a Niedokonane",
//     "Przyimki i Ich Użycie",
//     "Zaimki Osobowe i Zaimki Własne",
//     "Tryb Warunkowy i Wyrażenia Warunkowe",
//     "Czasowniki Zwrotne i Zaimki Zwrotne w Polskim"
//   ].indexed.map((item) => UnitGroup(item.$1, item.$2, generateUnits())).toList();
// }
//
// List<Unit> generateUnits() {
//   return List<int>.generate(10, (index) => index)
//       .map((e) => e + 1)
//       .map((index) => Unit(index, "Unit $index", false))
//       .toList();
// }
//
// class Rule {
//   final String title;
//   final String description;
//   final String example;
//
//   Rule({required this.title, required this.description, required this.example});
// }
//
//   // List of Rule objects
// List<Rule> mockMarkdownRules = [
//   Rule(
//     title: "### Present Tense: Regular Verbs",
//     description: "**Stress Pattern**: For regular verbs, the stress usually falls on the penultimate syllable.",
//     example: "Example: **mówić** (to speak), *rozumieć* (to understand)",
//   ),
//   Rule(
//     title: "### Present Tense: Irregular Verbs",
//     description: "**Stress Pattern**: Irregular verbs may have unpredictable stress patterns.",
//     example: "Example: **być** (to be), *mieć* (to have)",
//   ),
//   Rule(
//     title: "### Noun Declension: Masculine Nouns",
//     description: "**Nominative Case**: Masculine nouns often end in a consonant or a soft sign (ь) in the nominative case.",
//     example: "Example: **dom** (house), *student* (student)",
//   ),
//   Rule(
//     title: "### Noun Declension: Feminine Nouns",
//     description: "**Genitive Case**: Feminine nouns typically add '-y' or '-i' in the genitive case.",
//     example: "Example: **szkoła** (school), *książka* (book)",
//   ),
//   Rule(
//     title: "### Adjective Agreement",
//     description: "**Agreement with Noun**: Adjectives agree with nouns in gender, number, and case.",
//     example: "Example: **dobry dom** (good house), *dobra książka* (good book)",
//   ),
//   Rule(
//     title: "### Verb Conjugation: Regular Verbs",
//     description: "**Present Tense**: Regular verbs are conjugated based on their infinitive forms.",
//     example: "Example: **Ja mówię** (I speak), *Ty mówisz* (You speak)",
//   ),
//   Rule(
//     title: "### Verb Conjugation: Irregular Verbs",
//     description: "**Present Tense**: Irregular verbs have unique conjugation patterns.",
//     example: "Example: **Ja jestem** (I am), *Ty masz* (You have)",
//   ),
// ];
//
const mp3Url = "https://www.myinstants.com/media/sounds/grzegorz-brzeczyszczykiewicz.mp3";