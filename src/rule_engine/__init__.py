from dataclasses import dataclass
from typing import List, Dict, Any


@dataclass
class Location:
    block: str
    network: int
    index: int


@dataclass
class RuleResult:
    code: str
    severity: str
    message: str
    location: Location


# Type aliases for clarity
Instruction = str
Network = List[Instruction]
Block = Dict[str, Any]
