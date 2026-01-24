"""
Remove all GGA NMEA messages from a NMEA log file.

Usage:
    python RemoveGGA.py input_file [output_file]
    
If output_file is not specified, it will be created as <input_file>_NO_GGA.<input_file_ext>
"""

import sys
from pathlib import Path


def remove_gga_messages(input_file: str, output_file: str | None = None) -> None:
    """
    Remove all GGA NMEA messages from the input file.
    
    Args:
        input_file: Path to the input NMEA log file
        output_file: Path to the output file. If None, uses <input_file>_NO_GGA.<input_file_ext>
        
    Raises:
        FileNotFoundError: If input file does not exist
        IOError: If there are issues reading or writing files
    """

    print("Starting operation")

    input_path = Path(input_file)
    
    if not input_path.exists():
        raise FileNotFoundError(f"Input file not found: {input_file}")
    
    if output_file is None:
        output_path = input_path.parent / f"{input_path.stem}_NO_GGA{input_path.suffix}"
    else:
        output_path = Path(output_file)
    
    with open(input_path, 'r', encoding='utf-8') as infile:
        lines = infile.readlines()
    
    # Filter out GGA messages
    # GGA sentences match the pattern $xxGGA where xx is the GNSS system identifier
    filtered_lines = [line for line in lines if not ('GGA' in line and '$' in line and line.find('$') < line.find('GGA'))]
    
    with open(output_path, 'w', encoding='utf-8') as outfile:
        outfile.writelines(filtered_lines)
    
    removed_count = len(lines) - len(filtered_lines)
    print("Finished operation")
    print(f"Removed {removed_count} GGA message(s)")
    print(f"Output written to: {output_path}")


if __name__ == "__main__":
    if len(sys.argv) > 2:
        print(__doc__)
        sys.exit(1)
    
    input_file = sys.argv[1]
    output_file = sys.argv[2] if len(sys.argv) > 2 else None
    
    try:
        remove_gga_messages(input_file, output_file)
    except FileNotFoundError as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)
    except IOError as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)
