import re
from typing import List
from . import Block, RuleResult, Location


MESSAGES = {
    "R1": {
        "en": "Multiple assignments to same variable",
        "ro": "Atribuiri multiple pentru aceeași variabilă",
    },
    "R2": {
        "en": "TON/TOF without conditional IN",
        "ro": "TON/TOF fără IN condiționat",
    },
    "R3": {
        "en": "Constant signal assignment",
        "ro": "Semnal constant asignat",
    },
    "R4": {
        "en": "Movement/output without interlock",
        "ro": "Mişcare/iesire fără interlock",
    },
    "R5": {
        "en": "OB1 contains scan time warning",
        "ro": "OB1 conţine avertisment de timp de scanare",
    },
}


class Rule:
    def __init__(self, code: str, severity: str, func):
        self.code = code
        self.severity = severity
        self.func = func

    def __call__(self, block: Block, lang: str) -> List[RuleResult]:
        return self.func(block, lang)


# Rule implementations

def _r1(block: Block, lang: str) -> List[RuleResult]:
    results: List[RuleResult] = []
    for net_idx, network in enumerate(block.get("networks", []), start=1):
        seen = {}
        for idx, instr in enumerate(network, start=1):
            m = re.match(r"(\w+)\s*:=", instr)
            if m:
                var = m.group(1)
                if var in seen:
                    results.append(
                        RuleResult(
                            "R1",
                            "warning",
                            MESSAGES["R1"][lang],
                            Location(block["name"], net_idx, idx),
                        )
                    )
                else:
                    seen[var] = idx
    return results


def _r2(block: Block, lang: str) -> List[RuleResult]:
    results: List[RuleResult] = []
    for net_idx, network in enumerate(block.get("networks", []), start=1):
        for idx, instr in enumerate(network, start=1):
            if ("TON" in instr or "TOF" in instr) and re.search(r"IN:=\s*(TRUE|1)", instr, re.I):
                results.append(
                    RuleResult(
                        "R2",
                        "warning",
                        MESSAGES["R2"][lang],
                        Location(block["name"], net_idx, idx),
                    )
                )
    return results


def _r3(block: Block, lang: str) -> List[RuleResult]:
    results: List[RuleResult] = []
    for net_idx, network in enumerate(block.get("networks", []), start=1):
        for idx, instr in enumerate(network, start=1):
            if re.search(r":=\s*(TRUE|1)\b", instr, re.I):
                results.append(
                    RuleResult(
                        "R3",
                        "info",
                        MESSAGES["R3"][lang],
                        Location(block["name"], net_idx, idx),
                    )
                )
    return results


def _r4(block: Block, lang: str) -> List[RuleResult]:
    results: List[RuleResult] = []
    for net_idx, network in enumerate(block.get("networks", []), start=1):
        for idx, instr in enumerate(network, start=1):
            if re.match(r"(MOVE|OUT)", instr) and "IF" not in instr.upper():
                results.append(
                    RuleResult(
                        "R4",
                        "warning",
                        MESSAGES["R4"][lang],
                        Location(block["name"], net_idx, idx),
                    )
                )
    return results


def _r5(block: Block, lang: str) -> List[RuleResult]:
    results: List[RuleResult] = []
    if block.get("name") == "OB1" and block.get("meta", {}).get("scan_time"):
        results.append(
            RuleResult(
                "R5",
                "info",
                MESSAGES["R5"][lang],
                Location(block["name"], 0, 0),
            )
        )
    return results


RULES = [
    Rule("R1", "warning", _r1),
    Rule("R2", "warning", _r2),
    Rule("R3", "info", _r3),
    Rule("R4", "warning", _r4),
    Rule("R5", "info", _r5),
]
