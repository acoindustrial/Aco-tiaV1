from typing import List
from . import Block, RuleResult
from .rules import RULES


def analyze(blocks: List[Block], lang: str = "en") -> List[RuleResult]:
    results: List[RuleResult] = []
    for block in blocks:
        for rule in RULES:
            results.extend(rule(block, lang))
    return results
