import type { LogType } from "./log_type";
import type { TestCaseState } from "./test_case_state";

export interface TestLog {
  Lang: number;
  Message: string | null;
  Name: string | null;
  State: TestCaseState;
  Type: LogType;
}
